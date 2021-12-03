namespace App.Shell.State

open FSharpPlus
open App.Core.Domain
open App.Core.PluginDefinitions

/// Manages preferences state manipulation.
type public IPreferencesStateController =
    inherit IMutable
    abstract Preferences: Preferences

    abstract SetEnginesPathConfig: string -> SimpleResult
    abstract SetProjectsPathConfig: string -> SimpleResult
    abstract SetThemeConfig: Theme -> SimpleResult


/// Manages preferences state manipulation.
type internal PreferencesStateController
    (defaultPreferencesPlugin: UDefaultPreferences,
     persistPreferencesPlugin: UPersistPreferences) =

    let mutable prefs = defaultPreferencesPlugin ()
    let prefsChanged = Event<unit>()

    let threadSafe = threadSafeFactory ()

    let updateEnginesPath prefs dirData =
        prefs |> lens <@ prefs.General.EnginesPath.CurrentValue @> dirData

    let updateProjectsPath prefs dirData =
        prefs |> lens <@ prefs.General.ProjectsPath.CurrentValue @> dirData

    let updateTheme prefs newTheme =
        prefs |> lens <@ prefs.UI.Theme.CurrentValue @> newTheme

    let setPreferences newPrefs =
        if newPrefs <> prefs then
            persistPreferencesPlugin.Save newPrefs
            >>= (fun _ -> (prefs <- newPrefs) |> Ok)
            |> tap (fun _ -> prefsChanged.Trigger ())
        else Ok ()

    let setPreferencesThreadSafe newPrefs = threadSafe (fun () -> setPreferences newPrefs)

    interface IPreferencesStateController with

        member this.Preferences = prefs
        member this.StateChanged = prefsChanged.Publish

        member this.SetEnginesPathConfig fullPath =
            DirectoryData.tryCreate fullPath >>= updateEnginesPath prefs >>= setPreferencesThreadSafe

        member this.SetProjectsPathConfig fullPath =
            DirectoryData.tryCreate fullPath >>= updateProjectsPath prefs >>= setPreferencesThreadSafe

        member this.SetThemeConfig newTheme =
            newTheme |> updateTheme prefs >>= setPreferencesThreadSafe
