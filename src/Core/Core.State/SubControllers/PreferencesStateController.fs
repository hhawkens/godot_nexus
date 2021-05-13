namespace App.Core.State

open App.Core.PluginDefinitions
open App.Utilities

// TODO prefs serialization
type public PreferencesStateController (defaultPreferencesPlugin: UDefaultPreferences) =

    let mutable prefs = defaultPreferencesPlugin ()
    let prefsChanged = Event<unit>()

    let threadSafe = threadSafeFactory ()

    let setPreferences = function
        | newPrefs when newPrefs <> prefs ->
            prefs <- newPrefs
            prefsChanged.Trigger ()
        | _ -> ()

    member public this.Preferences = prefs
    member public this.PreferencesChanged = prefsChanged.Publish

    member public this.SetPreferences newPrefs =
        threadSafe (fun () -> setPreferences newPrefs)
