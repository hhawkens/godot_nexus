namespace App.Core.PluginDefinitions

open App.Core.Domain

type public LoadPreferencesResult =
    | Loaded of Preferences // Successfully loaded
    | LoadedDefaults of Preferences // No file, defaults loaded
    | LoadFailed of ErrorMessage // Error during load
