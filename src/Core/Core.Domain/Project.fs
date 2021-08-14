namespace App.Core.Domain

open System

type public Project = {
    Name: ProjectName
    Path: ProjectDirectory
    File: ProjectFile
    AssociatedEngine: EngineInstall option
} with
    member this.Id =
        Id.WithPrefix
            IdPrefixes.project
            (HashCode.Combine(this.Name, this.Path, this.AssociatedEngine) |> IdVal)
