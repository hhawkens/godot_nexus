namespace App.Core.Addons

open FSharpPlus
open App.Utilities

type internal AddonController () =

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

    interface IAddonController with

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
