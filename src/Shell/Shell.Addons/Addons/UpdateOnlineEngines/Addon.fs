module public App.Shell.Addons.UpdateOnlineEnginesAddon

open App.Core.Domain
open App.Shell.State

[<Literal>]
let internal GodotDownloadUrl = "https://downloads.tuxfamily.org/godotengine/"

let private updateOnlineEngines godotVersionQuery (appStateController: IAppStateController) = async {
    let! enginesResult = EnginesFinder.find godotVersionQuery GodotDownloadUrl
    match enginesResult with
    | Ok engines -> appStateController.EngineStateController.SetOnlineEngines engines
    | Error err -> appStateController.ThrowError (Error.General err)
}

let private addon = {
    Id = "Update Online Engines"; BeforeInitializeTask = None; AfterInitializeTask = None; TickTask = None
}

/// Update Engines Online Addon Instance (Linux Version)
let public linux = {
    addon with
        AfterInitializeTask = updateOnlineEngines GodotVersionQuery.linux |> AddonAsync |> Some
}

/// Update Engines Online Addon Instance (Windows Version)
let public windows = {
    addon with
        AfterInitializeTask = updateOnlineEngines GodotVersionQuery.windows |> AddonAsync |> Some
}

/// Update Engines Online Addon Instance (OSX Version)
let public mac = {
    addon with
        AfterInitializeTask = updateOnlineEngines GodotVersionQuery.mac |> AddonAsync |> Some
}
