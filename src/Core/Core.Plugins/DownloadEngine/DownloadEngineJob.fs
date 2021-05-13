namespace App.Core.Plugins

open System
open System.Threading
open App.Core.Domain
open App.Utilities

type public DownloadEngineJob (cacheDirectory: CacheDirectory, downloaderAsync) =

    let mutable statusMachine =
        JobStatusMachine.New<EngineZipFile * EngineOnline, ErrorMessage> ()

    let id =
        let idVal = Rand.NextI32() |> IdVal
        let prefixSub = nameof DownloadEngineJob |> stringToByte |> IdPrefixSub
        Id.WithPrefixSub IdPrefixes.job prefixSub idVal

    let cancelSource = new CancellationTokenSource()
    let updateEvent = Event<IJob> ()
    let threadSafe = threadSafeFactory ()

    member private this.progressHook actionName =
        let mutable updateCounter = 0uy
        let hook progress =
            updateCounter <- updateCounter + 1uy
            if updateCounter > 50uy then do
                updateCounter <- 0uy
                match progress with
                | UnknownProgress ->
                    this.SetStatus ({Action = actionName; Progress = None} |> Running)
                | DownloadProgress p ->
                    let percent = (float p.Current.bytes) / (float p.Total.bytes) |> Percent.FromFloat
                    this.SetStatus ({Action = actionName; Progress = percent} |> Running)
        hook

    member private this.SetStatusGeneric statusSetter newStatus =
        threadSafe (fun () ->
            match (statusSetter newStatus statusMachine) with
            | Some sm ->
                statusMachine <- sm
                updateEvent.Trigger this
            | None -> ())

    member private this.SetStatus newStatus =
        this.SetStatusGeneric JobStatusMachine<_,_>.Transition newStatus

    member private this.SetEndStatus endStatus =
        this.SetStatusGeneric JobStatusMachine<_,_>.Conclude endStatus

    member private this.Download engine uri downloadFolder cancelToken progressHook = async {
        let! downloadResult = downloaderAsync uri downloadFolder cancelToken progressHook
        match downloadResult with
        | Ok file -> this.SetEndStatus ((EngineZipFile file, engine) |> Succeeded)
        | Error msg -> this.SetEndStatus (Failed msg)
    }

    interface IDownloadEngineJob with

        member this.Id = id
        member this.Name = nameof DownloadEngineJob
        member this.Status = statusMachine.Status
        member this.EndStatus = statusMachine.EndStatus
        member this.Updated = updateEvent.Publish

        member this.Abort () =
            this.SetStatus Aborting
            cancelSource.Cancel()
            this.SetEndStatus Aborted

        member this.Run engine = async {
            let actionName = $"Downloading {engine}"
            this.SetStatus ({Action = actionName; Progress = None} |> Running)

            let downloadFolder = cacheDirectory.Val.FullPath |> DirectoryData.TryCreate
            let uri = exnToResult (fun () -> Uri engine.Url)
            let cancelToken = cancelSource.Token
            let progressHook = this.progressHook actionName

            match downloadFolder, uri with
            | Ok folder, Ok uri ->
                do! this.Download engine uri folder cancelToken progressHook
            | Error msg,_ | _, Error msg ->
                this.SetEndStatus (Failed msg)
        }
