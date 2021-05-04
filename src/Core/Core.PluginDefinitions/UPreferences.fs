namespace App.Core.PluginDefinitions

open App.Core.Domain

/// Saves and loads preferences on disk.
type public UPersistPreferences =
    inherit UPlugin
    abstract Save: Preferences -> Result<unit, string>
    abstract Load: unit -> LoadPreferencesResult

/// Defines default preferences for the current system.
type public UDefaultPreferences = unit -> Preferences
