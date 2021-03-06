module public App.Shell.Plugins.RemoveEngine

open App.Core.Domain
open App.Core.PluginDefinitions

let private removeEngine (engineInstall: EngineInstall) =
    match engineInstall.Directory.StillExists with
    | false -> NotFound
    | true ->
        match engineInstall.Directory.TryDelete () with
        | Ok _ -> SuccessfulRemoval
        | Error err -> RemovalFailed (err |> String.concat "\n")

let public plugin: URemoveEngine = removeEngine
