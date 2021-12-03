namespace App.Shell.Plugins

open System.IO
open App.Core.PluginDefinitions
open FSharpPlus

type internal PersistAppState () =

    [<Literal>]
    let file = "AppState.bin"

    let mutable appStateFile = SetOnce(FileData.Empty)

    interface UPersistAppState with

        member this.Load () =
            (fun _ -> AppStateSerializer.loadUnsafe appStateFile.Value.FullPath) |> exnToResult

        member this.Save appState =
            (fun _ -> AppStateSerializer.saveUnsafe appStateFile.Value.FullPath appState) |> exnToResult

        member this.TryInitialize () =
            match FileData.tryCreate (Path.Combine(AppDataPath, file)) with
            | Ok f -> appStateFile.SetOrFail(f) |> Ok
            | Error err -> Error err
