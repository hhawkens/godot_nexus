namespace App.Shell.State

open FSharpPlus
open App.Core.Domain
open App.Core.PluginDefinitions

/// Manages engines state manipulation.
type public IEngineStateController =
    abstract SetOnlineEngines: EngineOnline seq -> unit
    abstract InstallEngine: EngineOnline -> unit
    abstract RemoveEngine: EngineInstall -> SimpleResult
    abstract SetActiveEngine: EngineInstall -> SimpleResult
    abstract RunEngine: EngineInstall -> SimpleResult


/// Manages engines state manipulation.
type internal EngineStateController
    (enginesDirectoryPlugin: UEnginesDirectoryGetter,
     downloadEnginePlugin: UDownloadEngine,
     installEnginePlugin: UInstallEngine,
     removeEnginePlugin: URemoveEngine,
     runEnginePlugin: URunEngine,
     cachingPlugin: UCaching,
     jobsController: IJobsController,
     appStateInstance: AppStateInstance) =

    let enginesDir = enginesDirectoryPlugin()
    let errorOccurred = Event<Error>()

    let state () = appStateInstance.State
    let setState appState = appStateInstance.SetState appState

    let installDownloadedEngine engineZipFile engine =
        let installEngineJob = installEnginePlugin engineZipFile enginesDir engine
        jobsController.AddJob (InstallEngine installEngineJob)
        installEngineJob.Updated.Add (fun _ ->
            match installEngineJob.EndStatus with
            | Succeeded engineInstall ->
                setState {state() with EngineInstalls = state().EngineInstalls.Add engineInstall}
            | _ -> ())
        installEngineJob.Run () |> Async.StartChild |> ignore

    let engineIsInstalled (engine: EngineOnline) =
        state().EngineInstalls |> exists (fun ei -> ei.Data.Id = engine.Data.Id)

    member public this.ErrorOccurred = errorOccurred.Publish

    interface IEngineStateController with

        member this.SetOnlineEngines engines =
            let notInstalledEngines = engines |> filter (not << engineIsInstalled) |> Set
            setState {state() with EnginesOnline = notInstalledEngines}

        member this.InstallEngine engine =
            let downloadJob = downloadEnginePlugin cachingPlugin.CacheDirectory engine
            jobsController.AddJob (DownloadEngine downloadJob)
            downloadJob.Updated.Add (fun _ ->
                match downloadJob.EndStatus with
                | Succeeded (file, engineOnline) -> installDownloadedEngine file engineOnline.Data
                | _ -> ())
            downloadJob.Run () |> Async.StartChild |> ignore

        member this.RemoveEngine engineInstall =
            let rmFromState () = setState {state() with EngineInstalls = state().EngineInstalls.Remove engineInstall}
            match removeEnginePlugin engineInstall with
            | SuccessfulRemoval -> rmFromState () |> Ok
            | NotFound ->
                rmFromState ()
                Error($"Could not remove engine {engineInstall}, folder not found!")
            | RemovalFailed err -> Error($"Could not remove engine {engineInstall}, reason(s): {err}")

        member this.SetActiveEngine engineInstall =
            match state().EngineInstalls.SetActive engineInstall with
            | Some engineInstallsWithNewActive ->
                setState {state() with EngineInstalls = engineInstallsWithNewActive} |> Ok
            | None -> Error $"Cannot set engine {engineInstall} as active because it is not installed"

        member this.RunEngine engineInstall =
            runEnginePlugin engineInstall
