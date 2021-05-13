namespace App.Core.Domain

open System

/// Shows what kind of dotnet support, if any, an engine instance of Godot has.
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

    member public this.Id =
        Id.WithPrefix
            IdPrefixes.engine
            (HashCode.Combine(this.Version, this.DotNetSupport) |> IdVal)
