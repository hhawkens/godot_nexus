module internal App.Core.Addons.TextHelpers

open System
open System.Text.RegularExpressions

[<Literal>]
let internal ErrorMsg = "Could not find Godot engines online"

[<Literal>]
let internal GodotDownloadUrl = "https://downloads.tuxfamily.org/godotengine/"

let internal urlToVersion regex url =
    let groups = Regex.Match(url, regex).Groups
    if groups.Count > 1 then
        let versionTxt = groups.[1].Value
        let result, version = Version.TryParse versionTxt
        if result then Some version else None
    else
        None

let internal isGodotVersion url = Regex.IsMatch(url, @"\d\.\d")
let internal getGodotArchiveVersion url = urlToVersion @"Godot.v(.*)?.stable.x11.64.zip" url
let internal getGodotMonoArchiveVersion url = urlToVersion @"Godot.v(.*)?.stable.mono.x11.64.zip" url
