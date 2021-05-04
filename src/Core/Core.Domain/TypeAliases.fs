namespace App.Core.Domain

open App.Utilities

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

/// File that identifies a Godot project.
type public ProjectFile = ProjectFile of FileData with
    member this.Val =
        let (ProjectFile x) = this
        x

/// Directory where projects are being installed to.
type public ProjectsDirectory = ProjectsDirectory of DirectoryData with
    member this.Val =
        let (ProjectsDirectory x) = this
        x

/// Directory where temporary files are placed in.
type public CacheDirectory = CacheDirectory of DirectoryData with
    member this.Val =
        let (CacheDirectory x) = this
        x
