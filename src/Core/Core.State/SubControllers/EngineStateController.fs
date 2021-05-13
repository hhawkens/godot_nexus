namespace App.Core.State

open App.Core.Domain
open App.Core.PluginDefinitions

type public EngineStateController private
    (installEnginePlugin: UInstallEngine,
     removeEnginePlugin: URemoveEngine,
     runEnginePlugin: URunEngine,
     enginesDirectoryPlugin: UEnginesDirectoryGetter,
     appStateInstance: AppStateInstance) =

    let enginesDir = enginesDirectoryPlugin()
    let errorOccurred = Event<Error>()

    let state () = appStateInstance.State
    let setState appState = appStateInstance.SetState appState

    member public this.ErrorOccurred = errorOccurred.Publish

    member public this.InstallEngine engineZipFile engine =
        match installEnginePlugin engineZipFile enginesDir engine with
        | Ok engineInstall ->
            let newState = {state() with EngineInstalls = state().EngineInstalls.Add engineInstall}
            setState newState
        | Error err -> errorOccurred.Trigger (Error.general err)

    member this.RemoveEngine engineInstall =
            removeEnginePlugin engineInstall

    member this.SetActiveEngine engineInstall =
            match state().EngineInstalls.SetActive engineInstall with
            | Some newActive ->
                setState {state() with EngineInstalls = newActive} |> Ok
            | None -> Error $"Cannot set engine {engineInstall} as active because it is not installed"

    member this.RunEngine engineInstall =
            runEnginePlugin engineInstall

    static member public New
        installEnginePlugin
        removeEnginePlugin
        runEnginePlugin
        enginesDirectoryPlugin
        appStateInstance =
        EngineStateController (
            installEnginePlugin,
            removeEnginePlugin,
            runEnginePlugin,
            enginesDirectoryPlugin,
            appStateInstance)
