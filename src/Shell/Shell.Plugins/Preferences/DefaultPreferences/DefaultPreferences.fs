module internal App.Shell.Plugins.DefaultPreferences

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

[<Literal>]
let private enginesPath = "engines"

[<Literal>]
let private projectsPath = "projects"

let private enginesDirectoryInfo = Path.Combine(AppDataPath, enginesPath) |> DirectoryData.tryCreate |> unwrap
let private projectsDirectoryInfo = Path.Combine(AppDataPath, projectsPath) |> DirectoryData.tryCreate |> unwrap

let private defaultEnginesPath = {
    Description = "Directory the downloaded engines are saved to"
    DefaultValue = enginesDirectoryInfo
    CurrentValue = enginesDirectoryInfo
}

let private defaultProjectsPath = {
    Description = "Directory the application looks for projects at"
    DefaultValue = projectsDirectoryInfo
    CurrentValue = projectsDirectoryInfo
}

let private defaultGeneralSettings = {
    EnginesPath = defaultEnginesPath
    ProjectsPath = defaultProjectsPath
}

let private themeSettingWith theme = {
    Description = "Defines look of the application UI"
    DefaultValue = theme
    CurrentValue = theme
}

/// Default preferences for Unix operating systems (Linux, Mac, BSD).
let internal unix: UDefaultPreferences = (fun () -> {
    General = defaultGeneralSettings
    UI = {
        Theme = themeSettingWith Theme.System
    }
})

/// Default preferences for Microsoft Windows "operating systems".
let internal windows: UDefaultPreferences = (fun () -> {
    General = defaultGeneralSettings
    UI = {
        Theme = themeSettingWith Theme.Dark
    }
})
