namespace App.Shell.State

open FSharpPlus
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

/// Manages preferences state manipulation.
type public IPreferencesStateController =
    abstract Preferences: Preferences
    abstract PreferencesChanged: Preferences IEvent

    abstract SetEnginesPathConfig: string -> SimpleResult
    abstract SetProjectsPathConfig: string -> SimpleResult
    abstract SetThemeConfig: Theme -> SimpleResult


/// Manages preferences state manipulation.
type public PreferencesStateController
    (defaultPreferencesPlugin: UDefaultPreferences,
     persistPreferencesPlugin: UPersistPreferences) =

    let mutable prefs = defaultPreferencesPlugin ()
    let prefsChanged = Event<Preferences>()

    let threadSafe = threadSafeFactory ()

    let updateEnginesPath prefs dirData =
        prefs |> lens <@ prefs.General.EnginesPath.CurrentValue @> dirData

    let updateProjectsPath prefs dirData =
        prefs |> lens <@ prefs.General.ProjectsPath.CurrentValue @> dirData

    let updateTheme prefs newTheme =
        prefs |> lens <@ prefs.UI.Theme.CurrentValue @> newTheme

    let triggerEventIfFailed result =
        do match result with | Error _ -> prefsChanged.Trigger prefs | _ -> ()
        result

    let setPreferences = function
        | newPrefs when newPrefs <> prefs ->
            persistPreferencesPlugin.Save newPrefs |> Result.bind (fun _ ->
                prefs <- newPrefs
                prefsChanged.Trigger prefs |> Ok)
        | _ -> Ok ()
            |> triggerEventIfFailed

    let setPreferencesThreadSafe newPrefs = threadSafe (fun () -> setPreferences newPrefs)

    interface IPreferencesStateController with

        member this.Preferences = prefs
        member this.PreferencesChanged = prefsChanged.Publish

        member this.SetEnginesPathConfig fullPath =
            DirectoryData.tryCreate fullPath >>= updateEnginesPath prefs >>= setPreferencesThreadSafe

        member this.SetProjectsPathConfig fullPath =
            DirectoryData.tryCreate fullPath >>= updateProjectsPath prefs >>= setPreferencesThreadSafe

        member this.SetThemeConfig newTheme =
            newTheme |> updateTheme prefs >>= setPreferencesThreadSafe
