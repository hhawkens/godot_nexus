module public App.Shell.Plugins.ProjectsDirectoryGetter

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open FSharpPlus

[<Literal>]
let private projectsDirName = "projects"

let public plugin : UProjectsDirectoryGetter =
    let dir =
        Path.Combine(AppDataPath, projectsDirName)
        |> DirectoryData.tryCreate
        |> unwrap
        |> ProjectDirectory
    (fun () -> dir)
