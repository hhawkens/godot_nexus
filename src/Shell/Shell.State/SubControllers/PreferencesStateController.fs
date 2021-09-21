namespace App.Shell.State

open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

/// Manages preferences state manipulation.
type public IPreferencesStateController =
    abstract Preferences: Preferences
    abstract PreferencesChanged: unit IEvent
    abstract SetPreferences: Preferences -> SimpleResult


/// Manages preferences state manipulation.
type public PreferencesStateController
    (defaultPreferencesPlugin: UDefaultPreferences,
     persistPreferencesPlugin: UPersistPreferences) =

    let mutable prefs = defaultPreferencesPlugin ()
    let prefsChanged = Event<unit>()

    let threadSafe = threadSafeFactory ()

    let setPreferences = function
        | newPrefs when newPrefs <> prefs ->
            persistPreferencesPlugin.Save newPrefs |> Result.bind (fun _ ->
                prefs <- newPrefs
                prefsChanged.Trigger () |> Ok)
        | _ -> Ok ()

    interface IPreferencesStateController with

        member this.Preferences = prefs
        member this.PreferencesChanged = prefsChanged.Publish
        member this.SetPreferences newPrefs = threadSafe (fun () -> setPreferences newPrefs)
