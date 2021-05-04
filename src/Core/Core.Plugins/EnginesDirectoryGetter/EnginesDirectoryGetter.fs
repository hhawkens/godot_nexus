module public App.Core.Plugins.EnginesDirectoryGetter

open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

[<Literal>]
let private enginesDirName = "engines_01"

let public plugin : UEnginesDirectoryGetter =
    let dir = DirectoryData.TryCreate enginesDirName |> unwrap |> EnginesDirectory
    (fun () -> dir)
