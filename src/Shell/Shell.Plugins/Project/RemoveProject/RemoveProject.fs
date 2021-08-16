module public App.Shell.Plugins.RemoveProject

open App.Core.Domain
open App.Core.PluginDefinitions

let private removeProject project =
    let projectPath = project.Path.Val
    match projectPath.StillExists with
    | true ->
        match projectPath.TryDelete () with
        | Ok () -> SuccessfulRemoval
        | Error err -> err |> String.concat "\n" |> RemovalFailed
    | false -> NotFound

let public plugin: URemoveProject = removeProject
