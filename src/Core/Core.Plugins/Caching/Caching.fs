namespace App.Core.Plugins

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

type public Caching () =

    [<Literal>]
    let cacheDirName = "app_cache"

    let initializationError = SetOnce<string option>(None)

    let cacheDirectoryOption =
        match Path.Combine(AppDataPath, cacheDirName) |> DirectoryData.TryCreate with
        | Ok dir -> Some dir
        | Error err ->
            initializationError.SetOrFail(Some $"Could not create cache directory \"{cacheDirName}\, reason:\n{err}")
            None

    interface UCaching with

        member this.CacheDirectory =
            match cacheDirectoryOption with
            | Some dir -> CacheDirectory dir
            | None -> failwith $"Trying to access failed plugin \"{nameof Caching}\""

        member this.Clean() =
            let cacheDirectory = (this:>UCaching).CacheDirectory.Val
            cacheDirectory.TryDelete() |> Result.mapError formatSeq

        member this.TryInitialize() =
            match initializationError.Value with
            | None ->
                (this:>UCaching).Clean() |> ignore
                Ok ()
            | Some err -> Error err
