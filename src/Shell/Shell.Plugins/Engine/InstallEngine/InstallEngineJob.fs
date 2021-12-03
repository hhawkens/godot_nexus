namespace App.Shell.Plugins

open System.IO
open System.IO.Compression
open System.Text.RegularExpressions
open FSharpPlus
open App.Core.Domain

type public InstallEngineJob
    (engineZipFile: EngineZipFile,
     enginesDirectory: EnginesDirectory,
     engineData: EngineData,
     executableFileRegex: Regex) as this =

    let mutable statusMachine = ObservableJobStatusMachine<EngineInstall, ErrorMessage> (ThreadUnsafe, this)

    let id =
        let idVal = engineData.GetHashCode() |> IdVal
        let prefixSub = nameof InstallEngineJob |> stringToByte |> IdPrefixSub
        Id.WithPrefixSub IdPrefixes.job prefixSub idVal

    let tryCleanup dir = DirectoryData.tryFind dir >>= (DirectoryData.tryDelete >> Result.toOption) |> ignore
    let cleanupIfError dir (result: Result<_,_>) =
        if result.IsError then do tryCleanup dir
        result

    let extract (fromZip: FileData) (toDir: DirectoryData) =
        (fun () -> ZipFile.ExtractToDirectory (fromZip.FullPath, toDir.FullPath))
        |> exnToResult
        >>= (fun _ -> Ok toDir)

    let findGodotFileInDir (dir: DirectoryData) =
        let foundFiles =
            dir.FindFilesRecWhere (fun f -> f.Name.ToLower().Contains("godot"))
            |> filter (fun f -> executableFileRegex.Match(f.Name).Success)
            |> toList
        match foundFiles with
        | [file] -> Ok file
        | _ -> Error "Unable to find Godot executable!"

    let installationWorkflow (enginesDirectory: DirectoryData) zipFile = monad.plus' {
        let installDir = Path.Combine(enginesDirectory.FullPath, engineData.Version.ToString())
        tryCleanup installDir
        let! installDirData = DirectoryData.tryCreate installDir >>= extract zipFile |> cleanupIfError installDir
        let! godotFile = findGodotFileInDir installDirData |> cleanupIfError installDir
        return EngineInstall.New engineData installDirData godotFile
    }

    let install enginesDirectory zipFile =
        match installationWorkflow enginesDirectory zipFile with
        | Ok engineInstall -> engineInstall |> Succeeded |> statusMachine.SetEndStatus
        | Error err -> err |> Failed |> statusMachine.SetEndStatus

    let runPlugin () =
        statusMachine.SetStatus ({Action = $"Unpacking {engineData}"; Progress = None} |> Running)
        match (engineZipFile.Val.StillExists, enginesDirectory.Val.StillExists) with
        | true, true -> install enginesDirectory.Val engineZipFile.Val
        | false, _ -> "Could not find Godot zip file to unpack!" |> Failed |> statusMachine.SetEndStatus
        | _, false -> "Engines directory does not exist or is not accessible!" |> Failed |> statusMachine.SetEndStatus

    interface IInstallEngineJob with

        member this.Id = id
        member this.Name = nameof InstallEngineJob
        member this.Status = statusMachine.Status
        member this.EndStatus = statusMachine.EndStatus
        member this.Updated = statusMachine.Updated

        member this.Abort () = () // no practical implementation at the moment

        member this.Run () = async {
            if statusMachine.Status = Waiting then do runPlugin ()
        }
