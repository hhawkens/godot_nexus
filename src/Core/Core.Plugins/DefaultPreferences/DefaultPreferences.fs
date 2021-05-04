module internal App.Core.Plugins.DefaultPreferences

open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

[<Literal>]
let private enginesPath = "01_engines"

[<Literal>]
let private projectsPath = "01_projects"

let private defaultEnginesPath = {
    Description = "Directory the downloaded engines are saved to"
    DefaultValue = DirectoryData.TryCreate enginesPath |> unwrap
    CurrentValue = DirectoryData.TryCreate enginesPath |> unwrap
}

let private defaultProjectsPath = {
    Description = "Directory the application looks for projects at"
    DefaultValue = DirectoryData.TryCreate projectsPath |> unwrap
    CurrentValue = DirectoryData.TryCreate projectsPath |> unwrap
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
    Ui = {
        Theme = themeSettingWith Theme.System
    }
})

/// Default preferences for Microsoft Windows "operating systems".
let internal windows: UDefaultPreferences = (fun () -> {
    General = defaultGeneralSettings
    Ui = {
        Theme = themeSettingWith Theme.Dark
    }
})
