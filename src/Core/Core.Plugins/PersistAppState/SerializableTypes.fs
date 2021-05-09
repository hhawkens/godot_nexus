namespace App.Core.Plugins

open App.Core.Domain
open App.Utilities

type public SerializableEngine (data: EngineData, url: string, fileSize: FileSize) =
    member val public Data = data
    member val public Url = url
    member val public FileSize = fileSize


type public SerializableEngineInstall (data: EngineData, path: DirectoryData) =
    member val public Data = data
    member val public Path = path


type public SerializableEngineInstalls (set: Set<EngineInstall>, active: EngineInstall option) =
    member val public Set = set
    member val public Active = active
