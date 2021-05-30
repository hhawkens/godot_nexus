[<AutoOpen>]
module public App.Utilities.IODataExtensions

open System.IO
open FSharpPlus

/// Extension methods for DirectoryData.
type public DirectoryData with

    /// Gets the directory of given file.
    static member public Of (file: FileData) =
        Path.GetDirectoryName(file.fullPath) |> DirectoryData.New

    /// Returns the files contained in this directory.
    member public this.Files =
        Directory.GetFiles this.fullPath
        |> choose FileData.TryFind

    /// Tries to remove this directory + all files + sub-directories in it.
    member public this.TryDelete() =
        if not this.StillExists then
            Error [$"Could not remove directory \"{this.FullPath}\" as it no longer exists."]
        else
            let rec removeRec (dir: DirectoryData) =
                let fileErrors =
                    dir.Files
                    |>> (fun file -> file.TryDelete())
                    |> filter (fun res -> res.IsError)
                    |>> unwrapError
                    |> List.ofArray
                let subDirErrors = dir.SubDirectories |>> removeRec |> flatten |> List.ofSeq
                let recErrors = fileErrors @ subDirErrors
                match recErrors with
                | [] -> dir.Delete()
                | errs -> errs

            match removeRec this with
            | [] -> Ok ()
            | errs -> Error errs

    /// Find all files in this folder + sub-folders that match given predicate.
    member public this.FindFilesRecWhere predicate =
        let rec findFiles pred found (dir: DirectoryData) =
            let found = List.append (dir.Files |> filter pred |> toList) found
            let subDirs = dir.SubDirectories
            if subDirs.Length = 0 then
                found
            else
                subDirs
                |> fold (findFiles pred) found
        findFiles predicate [] this


/// Extension methods for FileData.
and public FileData with

     /// Creates new file and containing directories or points to existing file (preserving its contents).
    static member public TryCreate path =
        match FileData.TryFind path with
        | Some file -> Ok file
        | None ->
            try
                let dir = Path.GetDirectoryName path
                if dir.Trim() <> "" then do
                    DirectoryData.TryCreate dir |> unwrap |> ignore
                File.Create path |> dispose
                Ok <| FileData.New path
            with
                | ex -> Error ex.Message
