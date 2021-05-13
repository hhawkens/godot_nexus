namespace App.Core.State

open App.Core.Domain
open App.Core.PluginDefinitions
open App.Core.State

// TODO split up into domain specific sub-state-controllers
// TODO create another controller for initialization
// TODO bind error events from sub controllers
type internal AppStateController
    (defaultPreferencesPlugin: UDefaultPreferences,
     jobsController: JobsController,
     appStateInstance: AppStateInstance,
     engineStateController: EngineStateController,
     projectStateController: ProjectStateController) =

    let errorOccurred = Event<Error>()
    let prefsChanged = Event<unit>()

    let mutable prefs = defaultPreferencesPlugin ()

    let setPreferences = function
        | newPrefs when newPrefs <> prefs ->
            prefs <- newPrefs
            prefsChanged.Trigger ()
        | _ -> ()

    interface IAppStateController with

        member this.ErrorOccurred = errorOccurred.Publish
        member this.State = appStateInstance.State:>IAppState
        member this.StateChanged = appStateInstance.StateChanged
        member this.Preferences = prefs
        member this.PreferencesChanged = prefsChanged.Publish
        member this.JobStarted = jobsController.JobStarted

        member this.AbortJob id = jobsController.AbortJob id
        member this.ThrowError err = errorOccurred.Trigger err

        member this.SetPreferences newPrefs = setPreferences newPrefs

        member this.SetOnlineEngines engines = engineStateController.SetOnlineEngines engines
        member this.InstallEngine engine = engineStateController.InstallEngine engine
        member this.RemoveEngine engineInstall = engineStateController.RemoveEngine engineInstall
        member this.SetActiveEngine engineInstall = engineStateController.SetActiveEngine engineInstall
        member this.RunEngine engineInstall = engineStateController.RunEngine engineInstall

        member this.CreateNewProject name = projectStateController.CreateNewProject name
        member this.AddExistingProject file = projectStateController.AddExistingProject file
        member this.RemoveProject project = projectStateController.RemoveProject project
        member this.OpenProject project = projectStateController.OpenProject project
