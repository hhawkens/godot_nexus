module public App.Core.Addons.UpdateOnlineEnginesAddon

open App.Core.Domain
open App.Core.State

// TODO OS specific code

let private updateOnlineEngines (appStateManager: IAppStateManager) = async {
    let! enginesResult = EnginesFinder.find TextHelpers.GodotDownloadUrl
    match enginesResult with
    | Ok engines -> appStateManager.SetOnlineEngines engines
    | Error err -> appStateManager.ThrowError (Error.general err)
}

/// Update Engines Online Addon Instance
let public addon = {
    Id = "Update Online Engines"
    BeforeInitializeTask = None
    AfterInitializeTask = updateOnlineEngines |> AddonAsync |> Some
    TickTask = None
}
