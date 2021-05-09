namespace App.Core.Plugins

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

type internal PersistPreferences (defaultPreferences: Preferences) =

    [<Literal>]
    let file = "UserSettings.ini"

    let mutable configFile = SetOnce(FileData.Empty)

    interface UPersistPreferences with

        member this.Load() =
            ConfigSerializer.LoadFrom defaultPreferences configFile.Value

        member this.Save prefs =
            ConfigSerializer.Save configFile.Value prefs

        member this.TryInitialize() =
            match FileData.TryCreate (Path.Combine(AppDataPath, file)) with
            | Ok f -> configFile.SetOrFail(f) |> Ok
            | Error err -> Error err
