namespace App.Core.State

open App.Core.Domain
open App.Core.State

// TODO create another controller for AppStateController initialization
// TODO (maybe?) interfaces for sub controllers
type internal AppStateController
    (errorOccurred: Event<Error>,
     appStateInstance: AppStateInstance,
     jobsController: JobsController,
     engineStateController: EngineStateController,
     projectStateController: ProjectStateController,
     preferencesStateController: PreferencesStateController) =

    do
        engineStateController.ErrorOccurred.Add errorOccurred.Trigger

    interface IAppStateController with

        member this.ErrorOccurred = errorOccurred.Publish
        member this.State = appStateInstance.State:>IAppState
        member this.StateChanged = appStateInstance.StateChanged
        member this.Preferences = preferencesStateController.Preferences
        member this.PreferencesChanged = preferencesStateController.PreferencesChanged
        member this.JobStarted = jobsController.JobStarted

        member this.AbortJob id = jobsController.AbortJob id
        member this.ThrowError err = errorOccurred.Trigger err

        member this.SetPreferences newPrefs = preferencesStateController.SetPreferences newPrefs

        member this.SetOnlineEngines engines = engineStateController.SetOnlineEngines engines
        member this.InstallEngine engine = engineStateController.InstallEngine engine
        member this.RemoveEngine engineInstall = engineStateController.RemoveEngine engineInstall
        member this.SetActiveEngine engineInstall = engineStateController.SetActiveEngine engineInstall
        member this.RunEngine engineInstall = engineStateController.RunEngine engineInstall

        member this.CreateNewProject name = projectStateController.CreateNewProject name
        member this.AddExistingProject file = projectStateController.AddExistingProject file
        member this.RemoveProject project = projectStateController.RemoveProject project
        member this.OpenProject project = projectStateController.OpenProject project
