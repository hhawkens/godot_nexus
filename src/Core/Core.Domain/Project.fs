namespace App.Core.Domain

open System
open App.Utilities

type public Project = {
    Name: ProjectName
    Path: DirectoryData // TODO add FileData
    AssociatedEngine: EngineInstall option
} with
    member this.Id =
        Id.WithPrefix
            IdPrefixes.project
            (HashCode.Combine(this.Name, this.Path, this.AssociatedEngine) |> IdVal)

and public ProjectName = ProjectName of string with
    member this.Val =
        let (ProjectName x) = this
        x
