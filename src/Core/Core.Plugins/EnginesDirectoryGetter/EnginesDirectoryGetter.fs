module public App.Core.Plugins.EnginesDirectoryGetter

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

[<Literal>]
let private enginesDirName = "engines"

let public plugin : UEnginesDirectoryGetter =
    let dir =
        Path.Combine(AppDataPath, enginesDirName)
        |> DirectoryData.TryCreate
        |> unwrap
        |> EnginesDirectory
    (fun () -> dir)
