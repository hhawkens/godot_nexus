namespace App.Core.PluginDefinitions

open App.Core.Domain

/// Describes the possible outcomes of loading preferences.
type public LoadPreferencesResult =
    | Loaded of Preferences // Successfully loaded
    | LoadedDefaults of Preferences // No file, defaults loaded
    | LoadFailed of ErrorMessage // Error during load

/// Describes the possible outcomes of trying to remove an existing object (e.g. engine or project).
type public RemovalResult =
    | SuccessfulRemoval
    | NotFound
    | RemovalFailed of ErrorMessage
