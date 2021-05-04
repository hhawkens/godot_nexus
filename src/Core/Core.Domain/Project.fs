namespace App.Core.Domain

open App.Utilities

type public Project = {
    Id: Id
    Name: ProjectName
    Path: DirectoryData
    AssociatedEngine: EngineInstall option
}

and public ProjectName = ProjectName of string
