namespace App.Core.State

open App.Core.Domain
open App.Core.PluginDefinitions
open App.Core.State
open FSharpPlus

// TODO split up into domain specific sub-state-controllers
type internal AppStateController(cachingPlugin: UCaching,
    persistAppStatePlugin: UPersistAppState,
    persistPreferencesPlugin: UPersistPreferences,
    defaultPreferencesPlugin: UDefaultPreferences,

    jobsController: JobsController,
    appStateInstance: AppStateInstance,

    enginesDirectoryPlugin: UEnginesDirectoryGetter,
    downloadEnginePlugin: UDownloadEngine,
    installEnginePlugin: UInstallEngine,
    removeEnginePlugin: URemoveEngine,
    runEnginePlugin: URunEngine,

    projectsDirectoryPlugin: UProjectsDirectoryGetter,
    createNewProjectPlugin: UCreateNewProject,
    addExistingProjectPlugin: UAddExistingProject,
    removeProjectPlugin: URemoveProject,
    openProjectPlugin: UOpenProject) =

    let enginesDir = enginesDirectoryPlugin()
    let projectsDir = projectsDirectoryPlugin()
    let errorOccurred = Event<Error>()
    let prefsChanged = Event<unit>()

    let mutable prefs = defaultPreferencesPlugin ()

    let state() = appStateInstance.State
    let setState appState = appStateInstance.SetState appState

    let setPreferences = function
        | newPrefs when newPrefs <> prefs ->
            prefs <- newPrefs
            prefsChanged.Trigger ()
        | _ -> ()

    let addProject project =
        let newState = {state() with Projects = state().Projects.Add project}
        setState newState

    let removeProject project =
        let newState = {state() with Projects = state().Projects.Remove project}
        setState newState

    let installEngine engineZipFile engine =
        match installEnginePlugin engineZipFile enginesDir engine with
        | Ok engineInstall ->
            let newState = {state() with EngineInstalls = state().EngineInstalls.Add engineInstall}
            setState newState
        | Error err -> errorOccurred.Trigger (Error.general err)

    let engineIsInstalled (engine: Engine) =
        state().EngineInstalls |> exists (fun ei -> ei.Id = engine.Id)

    interface IAppStateController with

        member this.ErrorOccurred = errorOccurred.Publish
        member this.State = appStateInstance.State:>IAppState
        member this.StateChanged = appStateInstance.StateChanged
        member this.Preferences = prefs
        member this.PreferencesChanged = prefsChanged.Publish
        member this.JobStarted = jobsController.JobStarted

        member this.AbortJob id = jobsController.AbortJob id

        member this.ThrowError err =
            errorOccurred.Trigger err

        member this.SetPreferences newPrefs =
            setPreferences newPrefs

        member this.SetOnlineEngines engines =
            let notInstalledEngines = engines |> filter (not << engineIsInstalled) |> Set
            setState {state() with Engines = notInstalledEngines}

        member this.InstallEngine engine =
            let downloadJob = downloadEnginePlugin cachingPlugin.CacheDirectory
            jobsController.AddJob (DownloadEngine downloadJob)
            downloadJob.Updated.Add (fun _ ->
                match downloadJob.EndStatus with
                | Succeeded (file, engine) -> installEngine file engine
                | _ -> ())
            downloadJob.Run engine |> Async.StartChild |> ignore

        member this.RemoveEngine engineInstall =
            removeEnginePlugin engineInstall

        member this.SetActiveEngine engineInstall =
            match state().EngineInstalls.SetActive engineInstall with
            | Some newActive ->
                setState {state() with EngineInstalls = newActive} |> Ok
            | None -> Error $"Cannot set engine {engineInstall} as active because it is not installed"

        member this.RunEngine engineInstall =
            runEnginePlugin engineInstall

        member this.CreateNewProject name =
            let job = createNewProjectPlugin projectsDir
            jobsController.AddJob (CreateProject job)
            job.Run name |> Async.StartChild |> ignore

        member this.AddExistingProject file =
            match addExistingProjectPlugin file with
            | Ok project -> Ok (addProject project)
            | Error err -> Error err

        member this.RemoveProject project =
            match removeProjectPlugin project with
            | Ok _ -> Ok (removeProject project)
            | Error err -> Error err

        member this.OpenProject project =
            openProjectPlugin project
