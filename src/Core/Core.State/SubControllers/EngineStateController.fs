namespace App.Core.State

open FSharpPlus
open App.Core.Domain
open App.Core.PluginDefinitions

type public EngineStateController
    (enginesDirectoryPlugin: UEnginesDirectoryGetter,
     downloadEnginePlugin: UDownloadEngine,
     installEnginePlugin: UInstallEngine,
     removeEnginePlugin: URemoveEngine,
     runEnginePlugin: URunEngine,
     cachingPlugin: UCaching,
     jobsController: JobsController,
     appStateInstance: AppStateInstance) =

    let enginesDir = enginesDirectoryPlugin()
    let errorOccurred = Event<Error>()

    let state () = appStateInstance.State
    let setState appState = appStateInstance.SetState appState

    let installDownloadedEngine engineZipFile engine =
        match installEnginePlugin engineZipFile enginesDir engine with
        | Ok engineInstall ->
            let newState = {state() with EngineInstalls = state().EngineInstalls.Add engineInstall}
            setState newState
        | Error err -> errorOccurred.Trigger (Error.general err)

    let engineIsInstalled (engine: Engine) =
        state().EngineInstalls |> exists (fun ei -> ei.Id = engine.Id)

    member public this.ErrorOccurred = errorOccurred.Publish

    member public this.SetOnlineEngines engines =
        let notInstalledEngines = engines |> filter (not << engineIsInstalled) |> Set
        setState {state() with Engines = notInstalledEngines}

    member public this.InstallEngine engine =
        let downloadJob = downloadEnginePlugin cachingPlugin.CacheDirectory
        jobsController.AddJob (DownloadEngine downloadJob)
        downloadJob.Updated.Add (fun _ ->
            match downloadJob.EndStatus with
            | Succeeded (file, engine) -> installDownloadedEngine file engine
            | _ -> ())
        downloadJob.Run engine |> Async.StartChild |> ignore

    member public this.RemoveEngine engineInstall =
            removeEnginePlugin engineInstall

    member public this.SetActiveEngine engineInstall =
            match state().EngineInstalls.SetActive engineInstall with
            | Some newActive ->
                setState {state() with EngineInstalls = newActive} |> Ok
            | None -> Error $"Cannot set engine {engineInstall} as active because it is not installed"

    member public this.RunEngine engineInstall =
            runEnginePlugin engineInstall
