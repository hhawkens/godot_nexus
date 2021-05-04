namespace App.Utilities

open System.IO
open FSharpPlus

/// Immutable light weight alternative to System.IO.DirectoryInfo.
type public DirectoryData = private {
    fullPath: string
} with

    /// Returns the full path to this directory.
    member public this.FullPath = this.fullPath

    /// Returns folder name  of this directory.
    member public this.Name = Path.GetFileName this.fullPath

    /// Determines if the given path refers to an existing DirectoryInfo on disk.
    member public this.StillExists = Directory.Exists this.fullPath

    /// Retrieves the parent directory
    member public this.Parent =
        try
            Directory.GetParent(this.fullPath).FullName
                |> DirectoryData.New
                |> Ok
        with
            | ex -> Error ex.Message

    /// Returns the sub-directories contained within this directory.
    member public this.SubDirectories =
        Directory.GetDirectories this.fullPath
        |> choose DirectoryData.TryFind

    /// Gets the current working directory of the application.
    static member public Current = {fullPath = Directory.GetCurrentDirectory()}

    /// Returns a directory object for given path, if it exists on disk.
    static member public TryFind path =
        match Directory.Exists path with
            | true -> Some <| DirectoryData.New path
            | false -> None

    /// Tries to create a new folder on with given path.
    static member public TryCreate path =
        try
            Directory.CreateDirectory path |> ignore
            Ok <| DirectoryData.New path
        with
            | ex -> Error ex.Message

    static member internal New path =
        {fullPath = (DirectoryInfo path).FullName}

    member internal this.Delete () =
        try
            Directory.Delete this.fullPath
            []
        with | ex ->
            [ex.Message]
