namespace App.Core.State

open App.Core.Domain

type public IAppStateManager =
    abstract ErrorOccurred: IEvent<Error>

    abstract State: IAppState
    abstract StateChanged: AppStateChangedArgs IEvent

    abstract Preferences: Preferences
    abstract PreferencesChanged: unit IEvent

    abstract JobStarted: JobDef IEvent

    abstract AbortJob: Id -> SimpleResult
    abstract ThrowError: Error -> unit

    abstract SetPreferences: Preferences -> unit

    abstract SetOnlineEngines: Engine seq -> unit
    abstract InstallEngine: Engine -> unit
    abstract RemoveEngine: EngineInstall -> SimpleResult
    abstract SetActiveEngine: EngineInstall -> SimpleResult
    abstract RunEngine: EngineInstall -> SimpleResult

    abstract CreateNewProject: ProjectName -> unit
    abstract AddExistingProject: ProjectFile -> SimpleResult
    abstract RemoveProject: Project -> SimpleResult
    abstract OpenProject: Project -> SimpleResult
