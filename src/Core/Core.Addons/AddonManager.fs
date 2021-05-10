namespace App.Core.Addons

open App.Core.State
open FSharpPlus
open App.Core.Domain
open App.Utilities

/// Addons are extensions to the core that hook themselves into the application,
/// are called once or regularly.
type public IAddonHook =

    /// Registers an addon with the addon-event-system
    abstract RegisterAddon: IAddon -> unit


/// Manages + updates addon hooks.
type public IAddonManager =

    /// Needs to be called by the system before it is initialized.
    abstract CallBeforeInitialize: unit -> unit

    /// Needs to be called by the system right after it is initialized.
    abstract CallAfterInitialize: IAppStateController -> unit

    /// Updates time based hooks.
    abstract Update: Tick -> IAppStateController -> unit

    /// Closes all running addons
    abstract ShutDown: unit -> SimpleResult


type internal AddonManager () =

    let mutable addons = Set.empty
    let mutable tickerContainers: TickerContainer list = []
    let beforeInitialized = SetOnce<bool>(false)
    let afterInitialized = SetOnce<bool>(false)

    let callAddonTask param = function
        | AddonSync syncTask -> syncTask param
        | AddonAsync asyncTask -> Async.Start (asyncTask param)

    let callAllAddonTasks taskGetter param =
        addons
        |> choose taskGetter
        |> iter (callAddonTask param)

    interface IAddonHook with

        member this.RegisterAddon addon =
            addons <- addons.Add addon
            match addon.TickTask with
            | Some tickTask ->
                let tc = TickerContainer(tickTask)
                tickerContainers <- tc::tickerContainers
            | None -> ()

    interface IAddonManager with

        member this.CallBeforeInitialize () =
            if not beforeInitialized.Value then
                callAllAddonTasks (fun adn -> adn.BeforeInitializeTask) ()
                beforeInitialized.Set(true)

        member this.CallAfterInitialize appStateController =
            if not afterInitialized.Value then
                callAllAddonTasks (fun adn -> adn.AfterInitializeTask) appStateController
                afterInitialized.Set(true)

        member this.Update tick appStateController =
            for tickerContainer in tickerContainers do
                tickerContainer.Update tick appStateController

        member this.ShutDown() = failwith "todo"
