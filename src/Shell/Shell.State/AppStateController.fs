namespace App.Shell.State

open App.Core.Domain

// TODO create another controller for AppStateController initialization
// TODO check for 64 bit OS (32 bit not supported)
// TODO Integrity check after loading app state
type internal AppStateController
    (appStateInstance: AppStateInstance,
     jobsController: JobsController,
     engineStateController: EngineStateController,
     projectStateController: ProjectStateController,
     preferencesStateController: PreferencesStateController) =

    let errorOccurred = Event<Error> ()

    do
        engineStateController.ErrorOccurred.Add errorOccurred.Trigger

    interface IAppStateController with

        member _.ErrorOccurred = errorOccurred.Publish
        member _.State = appStateInstance.State:>IAppState
        member _.StateChanged = appStateInstance.StateChanged

        member this.EngineStateController = engineStateController:>IEngineStateController
        member this.JobsController = jobsController:>IJobsController
        member this.PreferencesStateController = preferencesStateController:>IPreferencesStateController
        member this.ProjectStateController = projectStateController:>IProjectStateController

        member _.ThrowError err = errorOccurred.Trigger err
