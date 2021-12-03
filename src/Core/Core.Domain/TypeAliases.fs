namespace App.Core.Domain

open FSharpPlus

/// Represents compressed engine files.
type public EngineZipFile = EngineZipFile of FileData with
    member this.Val =
        let (EngineZipFile x) = this
        x

/// Directory where engines are being installed to.
type public EnginesDirectory = EnginesDirectory of DirectoryData with
    member this.Val =
        let (EnginesDirectory x) = this
        x

/// Name of a Godot project.
type public ProjectName = ProjectName of string with
    member this.Val =
        let (ProjectName x) = this
        x

/// File that contains a Godot project definition.
type public ProjectFile = ProjectFile of FileData with
    member this.Val =
        let (ProjectFile x) = this
        x

/// Directory where projects are being installed to.
type public ProjectDirectory = ProjectDirectory of DirectoryData with
    member this.Val =
        let (ProjectDirectory x) = this
        x

/// Directory where temporary files are placed in.
type public CacheDirectory = CacheDirectory of DirectoryData with
    member this.Val =
        let (CacheDirectory x) = this
        x
