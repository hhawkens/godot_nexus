namespace FSharpPlus

open System.IO

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
        let tryFind path =
            match Directory.Exists path with
                | true -> Some <| DirectoryData.New path
                | false -> None
        Directory.GetDirectories this.fullPath
        |> choose tryFind

    static member internal New path = {fullPath = (DirectoryInfo path).FullName}

    /// Returns the files contained in this directory.
    member public this.Files =
        Directory.GetFiles this.fullPath
        |> choose FileData.tryFind

    /// Tries to remove this directory + all files + sub-directories in it.
    member public this.TryDelete() =
        if not this.StillExists then
            Error [$"Could not remove directory \"{this.FullPath}\" as it no longer exists."]
        else
            let rec removeRec (dir: DirectoryData) =
                let fileErrors =
                    dir.Files
                    |>> FileData.tryDelete
                    |> filter Result.isError
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

    member internal this.Delete () =
        try
            Directory.Delete this.fullPath
            []
        with | ex ->
            [ex.Message]


module public DirectoryData =

    /// Gets the current working directory of the application.
    let public current () = {fullPath = Directory.GetCurrentDirectory()}

    /// Returns a directory object for given path, if it exists on disk.
    let public tryFind path =
        match Directory.Exists path with
            | true -> Some <| DirectoryData.New path
            | false -> None

    /// Tries to create a new folder on with given path.
    let public tryCreate path =
        try
            Directory.CreateDirectory path |> ignore
            Ok <| DirectoryData.New path
        with
            | ex -> Error ex.Message

    /// Gets the directory of given file.
    let public from (file: FileData) = Path.GetDirectoryName(file.fullPath) |> DirectoryData.New

    /// Tries to remove this directory + all files + sub-directories in it.
    let public tryDelete (directoryData: DirectoryData) = directoryData.TryDelete ()

    /// Find all files in this folder + sub-folders that match given predicate.
    let public findFilesRecWhere predicate (directoryData: DirectoryData) = directoryData.FindFilesRecWhere predicate
