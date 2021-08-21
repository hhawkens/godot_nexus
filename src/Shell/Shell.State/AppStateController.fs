namespace App.Shell.State

open App.Core.Domain

// TODO create another controller for AppStateController initialization
// TODO check for 64 bit OS (32 bit not supported)
// TODO Integrity check after loading app state
type internal AppStateController
    (errorOccurred: Event<Error>,
     appStateInstance: AppStateInstance,
     jobsController: JobsController,
     engineStateController: EngineStateController,
     projectStateController: ProjectStateController,
     preferencesStateController: PreferencesStateController) as this =

    let iThis = this:>IAppStateController

    do
        engineStateController.ErrorOccurred.Add errorOccurred.Trigger

    interface IAppStateController with

        member _.ErrorOccurred = errorOccurred.Publish
        member _.State = appStateInstance.State:>IAppState
        member _.StateChanged = appStateInstance.StateChanged
        member _.Preferences = preferencesStateController.Preferences
        member _.PreferencesChanged = preferencesStateController.PreferencesChanged
        member _.JobStarted = jobsController.JobStarted

        member _.AbortJob id = jobsController.AbortJob id
        member _.ThrowError err = errorOccurred.Trigger err

        member _.SetPreferences newPrefs = preferencesStateController.SetPreferences newPrefs

        member _.SetOnlineEngines engines = engineStateController.SetOnlineEngines engines
        member _.InstallEngine engine = engineStateController.InstallEngine engine
        member _.RemoveEngine engineInstall = engineStateController.RemoveEngine engineInstall
        member _.SetActiveEngine engineInstall = engineStateController.SetActiveEngine engineInstall
        member _.RunEngine engineInstall = engineStateController.RunEngine engineInstall

        member _.CreateNewProject name = projectStateController.CreateNewProject name
        member _.AddExistingProject file = projectStateController.AddExistingProject file
        member _.RemoveProject project = projectStateController.RemoveProject project

        member _.OpenProject project =
            match project.AssociatedEngine, iThis.State.EngineInstalls.Active with
            | Some engine, _ | _, Some engine -> projectStateController.OpenProject engine project
            | None, None -> Error $"Cannot open {project} because no engine is installed!"
