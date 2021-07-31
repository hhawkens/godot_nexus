namespace App.Shell.Plugins

open System
open System.Threading
open App.Core.Domain
open App.Utilities

type public DownloadEngineJob
    (cacheDirectory: CacheDirectory,
     engineOnline: EngineOnline,
     downloaderAsync) as this =

    let mutable statusMachine =
        ObservableJobStatusMachine<EngineZipFile * EngineOnline, ErrorMessage> (ThreadSafe, this)

    let id =
        let idVal = Rand.NextI32() |> IdVal
        let prefixSub = nameof DownloadEngineJob |> stringToByte |> IdPrefixSub
        Id.WithPrefixSub IdPrefixes.job prefixSub idVal

    let cancelSource = new CancellationTokenSource()

    let progressHook actionName =
        let mutable updateCounter = 0uy
        let hook progress =
            updateCounter <- updateCounter + 1uy
            if updateCounter > 50uy then do
                updateCounter <- 0uy
                match progress with
                | UnknownProgress ->
                    statusMachine.SetStatus ({Action = actionName; Progress = None} |> Running)
                | DownloadProgress p ->
                    let percent = (float p.Current.bytes) / (float p.Total.bytes) |> Percent.FromFloat
                    statusMachine.SetStatus ({Action = actionName; Progress = percent} |> Running)
        hook

    let download engine uri downloadFolder cancelToken progressHook = async {
        let! downloadResult = downloaderAsync uri downloadFolder cancelToken progressHook
        match downloadResult with
        | Ok file -> statusMachine.SetEndStatus ((EngineZipFile file, engine) |> Succeeded)
        | Error msg -> statusMachine.SetEndStatus (Failed msg)
    }

    let runPlugin () = async {
        let actionName = $"Downloading {engineOnline}"
        statusMachine.SetStatus ({Action = actionName; Progress = None} |> Running)

        let downloadFolder = cacheDirectory.Val.FullPath |> DirectoryData.tryCreate
        let uri = exnToResult (fun () -> Uri engineOnline.Url)
        let progressHook = progressHook actionName

        match downloadFolder, uri with
        | Ok folder, Ok uri ->
            do! download engineOnline uri folder cancelSource.Token progressHook
        | Error msg,_ | _, Error msg ->
            statusMachine.SetEndStatus (Failed msg)
    }

    interface IDownloadEngineJob with

        member this.Id = id
        member this.Name = nameof DownloadEngineJob
        member this.Status = statusMachine.Status
        member this.EndStatus = statusMachine.EndStatus
        member this.Updated = statusMachine.Updated

        member this.Abort () =
            statusMachine.SetStatus Aborting
            cancelSource.Cancel()
            statusMachine.SetEndStatus Aborted

        member this.Run () = async {
            if statusMachine.Status = Waiting then do! runPlugin ()
        }
