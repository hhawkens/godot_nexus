namespace App.Core.Domain

open App.Utilities

/// Engine version of Godot that has not been installed yet (but can be downloaded online).
type public EngineOnline = private {
    data: EngineData
    url: string
    fileSize: FileSize
} with

    member this.Data = this.data
    member this.Url = this.url
    member this.FileSize = this.fileSize

    override this.ToString() = $"Godot{this.data}"

    static member New data url fileSize = {
        data = data
        url = url
        fileSize = fileSize
    }

/// Engine version of Godot that has been installed.
type public EngineInstall = private {
    data: EngineData
    path: DirectoryData
} with

    member this.Data = this.data
    member this.Path = this.path

    override this.ToString() = $"Godot{this.data}"

    static member New data path = {
        data = data
        path = path
    }
