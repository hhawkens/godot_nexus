namespace App.Core.Plugins

open System.IO
open System.IO.Compression
open FSharpPlus
open App.Core.Domain
open App.Utilities

type public InstallEngineJob
    (engineZipFile: EngineZipFile,
     enginesDirectory: EnginesDirectory,
     engineOnline: EngineOnline) =

    let mutable statusMachine = JobStatusMachine.New<EngineInstall, ErrorMessage> ()

    let id =
        let idVal = Rand.NextI32() |> IdVal
        let prefixSub = nameof InstallEngineJob |> stringToByte |> IdPrefixSub
        Id.WithPrefixSub IdPrefixes.job prefixSub idVal

    let updated = Event<IJob> ()

    let extract (fromZip: FileData) (toDir: DirectoryData) =
        (fun () -> ZipFile.ExtractToDirectory (fromZip.FullPath, toDir.FullPath))
        |> exnToResult
        |> Result.bind (fun _ -> Ok toDir)

    let findGodotFileInDir dir = Ok FileData.Empty // TODO

    let installationWorkflow (engineOnline: EngineOnline) (enginesDirectory: DirectoryData) zipFile = result {
        // TODO cleanup before creating folder
        let installDir = Path.Combine(enginesDirectory.FullPath, engineOnline.ToString())
        let! installDirData = DirectoryData.TryCreate installDir >>= extract zipFile
        let! godotFile = findGodotFileInDir installDirData
        return EngineInstall.New engineOnline.Data installDirData godotFile
    }

    // TODO "ObservableStatusMachine"
    member private this.SetStatusGeneric setter newStatus =
        match setter newStatus statusMachine with
            | Some sm ->
                statusMachine <- sm
                updated.Trigger this
            | None -> ()

    member private this.SetStatus newStatus =
        this.SetStatusGeneric JobStatusMachine<_,_>.Transition newStatus

    member private this.SetEndStatus endStatus =
        this.SetStatusGeneric JobStatusMachine<_,_>.Conclude endStatus

    member private this.Install engineOnline enginesDirectory zipFile =
        match installationWorkflow engineOnline enginesDirectory zipFile with
        | Ok engineInstall -> engineInstall |> Succeeded |> this.SetEndStatus
        | Error err -> err |> Failed |> this.SetEndStatus

    interface IInstallEngineJob with

        member this.Id = id
        member this.Name = nameof InstallEngineJob
        member this.Status = statusMachine.Status
        member this.EndStatus = statusMachine.EndStatus
        member this.Updated = updated.Publish

        member this.Abort () = () // no practical implementation of this possible at the moment

        member this.Run () = async {
            this.SetStatus ({Action = $"Unpacking {engineOnline}"; Progress = None} |> Running)
            match (engineZipFile.Val.StillExists, enginesDirectory.Val.StillExists) with
            | true, true -> this.Install engineOnline enginesDirectory.Val engineZipFile.Val
            | false, _ -> "Could not find Godot zip file to unpack!" |> Failed |> this.SetEndStatus
            | _, false -> "Engines directory does not exist or is not accessible!" |> Failed |> this.SetEndStatus
        }
