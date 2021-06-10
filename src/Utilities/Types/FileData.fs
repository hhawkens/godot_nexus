namespace App.Utilities

open System.IO
open FSharpPlus

/// Immutable light weight alternative to System.IO.FileInfo.
type public FileData = private {
    fullPath: string
} with

    /// Returns an invalid file data with an empty path.
    static member public Empty = {fullPath = ""}

    /// Returns the full path + extension of this file.
    member public this.FullPath = this.fullPath

    /// Returns the name + extension parts of this file.
    member public this.Name = Path.GetFileName this.fullPath

    /// Returns the extension of the given path. The returned value includes the period (".").
    member public this.Extension = Path.GetExtension this.fullPath

    /// Tests whether a file exists.
    member public this.StillExists = File.Exists this.fullPath

    /// Tries to delete this file.
    member public this.TryDelete() =
        if this.StillExists then
            try
                File.Delete this.FullPath
                Ok ()
            with | ex ->
                Error ex.Message
        else
            Error $"Could not delete file {this.FullPath} as it no longer exists."

    static member internal New path = {fullPath = (FileInfo path).FullName}



module public FileData =

    /// Points to existing file.
    let public tryFind path =
        match File.Exists path with
        | true -> Some <| FileData.New path
        | false -> None

    /// Creates new file and containing directories or points to existing file (preserving its contents).
    let public tryCreate path =
        match tryFind path with
        | Some file -> Ok file
        | None ->
            try
                let dir = Path.GetDirectoryName path
                if dir.Trim() <> "" then do
                    Directory.CreateDirectory dir |> ignore
                File.Create path |> dispose
                Ok <| FileData.New path
            with
                | ex -> Error ex.Message

    /// Tries to delete this file.
    let public tryDelete (fileData: FileData) = fileData.TryDelete ()
