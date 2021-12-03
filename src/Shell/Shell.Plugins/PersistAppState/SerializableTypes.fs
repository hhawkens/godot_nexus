namespace App.Shell.Plugins

open App.Core.Domain
open FSharpPlus

type public SerializableEngine (data: EngineData, url: string, fileSize: FileSize) =
    member val public Data = data
    member val public Url = url
    member val public FileSize = fileSize


type public SerializableEngineInstall (data: EngineData, directory: DirectoryData, executableFile: FileData) =
    member val public Data = data
    member val public Directory = directory
    member val public ExecutableFile = executableFile


type public SerializableEngineInstalls (set: Set<EngineInstall>, active: EngineInstall option) =
    member val public Set = set
    member val public Active = active
