namespace App.Shell.State

open App.Core.PluginDefinitions
open App.Utilities

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

    member public this.Preferences = prefs
    member public this.PreferencesChanged = prefsChanged.Publish

    member public this.SetPreferences newPrefs =
        threadSafe (fun () -> setPreferences newPrefs)
