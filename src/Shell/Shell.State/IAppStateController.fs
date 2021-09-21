namespace App.Shell.State

open App.Core.Domain

type public IAppStateController =
    abstract ErrorOccurred: IEvent<Error>
    abstract State: IAppState
    abstract StateChanged: AppStateChangedArgs IEvent

    abstract JobsController: IJobsController
    abstract EngineStateController: IEngineStateController
    abstract ProjectStateController: IProjectStateController
    abstract PreferencesStateController: IPreferencesStateController

    abstract ThrowError: Error -> unit
