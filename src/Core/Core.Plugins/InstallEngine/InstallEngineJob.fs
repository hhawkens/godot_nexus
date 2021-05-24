namespace App.Core.Plugins

open System.IO
open System.IO.Compression
open System.Text.RegularExpressions
open FSharpPlus
open App.Core.Domain
open App.Utilities

// TODO os specific plugin
type public InstallEngineJob
    (engineZipFile: EngineZipFile,
     enginesDirectory: EnginesDirectory,
     engineData: EngineData,
     executableFileRegex: Regex) as this =

    let mutable statusMachine = ObservableJobStatusMachine<EngineInstall, ErrorMessage> (ThreadUnsafe, this)

    let id =
        let idVal = Rand.NextI32() |> IdVal
        let prefixSub = nameof InstallEngineJob |> stringToByte |> IdPrefixSub
        Id.WithPrefixSub IdPrefixes.job prefixSub idVal

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

    let installationWorkflow (enginesDirectory: DirectoryData) zipFile = result {
        // TODO check and cleanup before creating folder
        let installDir = Path.Combine(enginesDirectory.FullPath, engineData.ToString())
        let! installDirData = DirectoryData.TryCreate installDir >>= extract zipFile
        let! godotFile = findGodotFileInDir installDirData
        return EngineInstall.New engineData installDirData godotFile
    }

    let install enginesDirectory zipFile =
        match installationWorkflow enginesDirectory zipFile with
        | Ok engineInstall -> engineInstall |> Succeeded |> statusMachine.SetEndStatus
        | Error err -> err |> Failed |> statusMachine.SetEndStatus

    interface IInstallEngineJob with

        member this.Id = id
        member this.Name = nameof InstallEngineJob
        member this.Status = statusMachine.Status
        member this.EndStatus = statusMachine.EndStatus
        member this.Updated = statusMachine.Updated

        member this.Abort () = () // no practical implementation at the moment

        member this.Run () = async {
            // TODO remove extracted files after failure
            statusMachine.SetStatus ({Action = $"Unpacking {engineData}"; Progress = None} |> Running)
            match (engineZipFile.Val.StillExists, enginesDirectory.Val.StillExists) with
            | true, true -> install enginesDirectory.Val engineZipFile.Val
            | false, _ -> "Could not find Godot zip file to unpack!" |> Failed |> statusMachine.SetEndStatus
            | _, false -> "Engines directory does not exist or is not accessible!" |> Failed |> statusMachine.SetEndStatus
        }
