module public App.Shell.Plugins.RunEngine

open System.Diagnostics
open App.Core.Domain
open App.Core.PluginDefinitions

let private runEngineProcess (engine: EngineInstall) =
    try
        let startInfo = ProcessStartInfo();
        startInfo.UseShellExecute <- true
        startInfo.FileName <- engine.ExecutableFile.FullPath
        startInfo.WindowStyle <- ProcessWindowStyle.Hidden

        let proc = Process.Start(startInfo)
        if proc <> null then Ok() else Error "Could not find engine process to start!"
    with | exn ->
        Error $"Error trying to run engine process:\n{exn.Message}"

let public plugin: URunEngine = runEngineProcess
