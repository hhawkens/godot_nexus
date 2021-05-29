namespace App.Core.Addons

open System
open System.Text.RegularExpressions
open FSharpPlus

[<AutoOpen>]
module private Common =

    let internal urlToVersion regex url =
        let groups = Regex.Match(url, regex).Groups
        if groups.Count > 1 then
            let versionTxt = groups.[1].Value
            let result, version = Version.TryParse versionTxt
            if result then Some version else None
        else
            None

    let internal isGodotVersion url = Regex.IsMatch(url, @"\d\.\d")


module internal GodotVersionQuery =

    let internal linux = { new IGodotVersionQuery with
        member this.IsVersion url = isGodotVersion url
        member this.GetArchiveVersion url = urlToVersion @"Godot.v(.*)?.stable.x11.64.zip" url
        member this.GetMonoArchiveVersion url = urlToVersion @"Godot.v(.*)?.stable.mono.x11.64.zip" url
    }

    let internal windows = { new IGodotVersionQuery with
        member this.IsVersion url = isGodotVersion url
        member this.GetArchiveVersion url = urlToVersion @"Godot.v(.*)?.stable.win64.exe.zip" url
        member this.GetMonoArchiveVersion url = urlToVersion @"Godot.v(.*)?.stable.mono.win64.zip" url
    }

    let internal mac = { new IGodotVersionQuery with
        member this.IsVersion url = isGodotVersion url

        member this.GetArchiveVersion url = monad.plus' {
            return! urlToVersion @"Godot.v(.*)?.stable.osx.universal.zip" url
            return! urlToVersion @"Godot.v(.*)?.stable.osx.fat.zip" url
            return! urlToVersion @"Godot.v(.*)?.stable.osx64.zip" url
        }

        member this.GetMonoArchiveVersion url = monad.plus' {
            return! urlToVersion @"Godot.v(.*)?.stable.mono.osx.64.zip" url
            return! urlToVersion @"Godot.v(.*)?.stable.mono.osx.fat.zip" url
            return! urlToVersion @"Godot.v(.*)?.stable.mono.osx64.zip" url
        }
    }
