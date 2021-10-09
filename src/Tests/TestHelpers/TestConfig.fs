module public App.TestHelpers.TestConfig

open System.IO
open App.Core.Domain
open App.Utilities

[<Literal>]
let private enginesPath = "engines"

[<Literal>]
let private projectsPath = "projects"

let private enginesDirectoryInfo = Path.Combine("appdata", enginesPath) |> DirectoryData.tryCreate |> unwrap
let private projectsDirectoryInfo = Path.Combine("appdata", projectsPath) |> DirectoryData.tryCreate |> unwrap

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

let private themeSettings = {
    Description = "Defines look of the application UI"
    DefaultValue = Theme.System
    CurrentValue = Theme.System
}

let public preferences = {
    General = defaultGeneralSettings
    UI = { Theme = themeSettings }
}

let public withEnginePath newPath (prefs: Preferences) =
    {prefs with General = {prefs.General with EnginesPath = {prefs.General.EnginesPath with CurrentValue = DirectoryData.tryCreate newPath |> unwrap}}}
