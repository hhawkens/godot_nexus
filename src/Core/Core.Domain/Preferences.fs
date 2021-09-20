namespace App.Core.Domain

open App.Utilities

type public Preferences = {
    General: GeneralPreferences
    UI: UiPreferences
}

// -------------- Sections and Settings ----------------

and public GeneralPreferences = {
    EnginesPath: DirectoryData ConfigData
    ProjectsPath: DirectoryData ConfigData
}

and public UiPreferences = {
    Theme: Theme ConfigData
}
