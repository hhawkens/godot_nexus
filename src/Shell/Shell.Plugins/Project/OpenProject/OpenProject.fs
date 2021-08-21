module public App.Shell.Plugins.OpenProject

open System.Diagnostics
open FSharpPlus
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

let private getProcessStartInfoToOpenProject (engineInstall: EngineInstall) (project: Project) =
    match engineInstall.ExecutableFile.StillExists, project.File.Val.StillExists with
    | true, true ->
        let processStartInfo = ProcessStartInfo()
        processStartInfo.FileName <- engineInstall.ExecutableFile.FullPath
        processStartInfo.Arguments <- project.File.Val.FullPath
        Ok processStartInfo
    | true, false -> Error $"Project file for {project} no longer exists!"
    | false, true -> Error $"Executable of {engineInstall} not found!"
    | false, false -> Error $"Could not find neither executable of {engineInstall} nor project file of {project}!"

let private startProcess startInfo =
    use proc = new Process()
    proc.StartInfo <- startInfo
    exnToResult (fun _ -> proc.Start() |> ignore)

let private formatProcessResult project = function
    | Error err -> Error $"Failed to open project {project}, reason:\n{err}"
    | ok -> ok

let private openProjectWith engineInstall project =
    getProcessStartInfoToOpenProject engineInstall project
    >>= startProcess
    |> formatProcessResult project

let public plugin: UOpenProject = openProjectWith
