namespace App.Core.Domain

open System
open App.Utilities

/// Shows what kind of dotnet support, if any, an engine variation of Godot has.
[<Struct>]
type public DotNetSupport =
    | NoSupport
    | Mono
    | DotNetCore
with

    override this.ToString() =
        match this with
        | NoSupport -> ""
        | Mono -> nameof Mono
        | DotNetCore -> ".NET Core"

/// Fundamental info about a godot engine instance.
type public EngineData = {
    Version: Version
    DotNetSupport: DotNetSupport
} with

    override this.ToString() =
        let dotnetSupport = this.DotNetSupport.ToString()
        let dotnetText = if dotnetSupport <> "" then $" ({dotnetSupport})" else ""
        $"v{this.Version}{dotnetText}"

/// Represents an engine version of Godot that has not been installed yet.
and public Engine = private {
    id: Id
    data: EngineData
    url: string
    fileSize: FileSize
} with

    member this.Id = this.id
    member this.Data = this.data
    member this.Url = this.url
    member this.FileSize = this.fileSize

    override this.ToString() = $"Godot{this.data}"

    static member New data url fileSize = {
        id = (IdPrefixes.engine, HashCode.Combine(data, url) |> IdVal) ||> Id.WithPrefix
        data = data
        url = url
        fileSize = fileSize
    }

/// Represents an engine version of Godot that has been installed.
and public EngineInstall = private {
    id: Id
    data: EngineData
    path: DirectoryData
} with

    member this.Id = this.id
    member this.Data = this.data
    member this.Path = this.path

    override this.ToString() = $"Godot{this.data}"

    static member New data path = {
        id = (IdPrefixes.engineInstall, HashCode.Combine(data, path) |> IdVal) ||> Id.WithPrefix
        data = data
        path = path
    }
