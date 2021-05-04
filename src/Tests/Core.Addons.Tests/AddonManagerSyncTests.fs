module public App.Core.Addons.Tests.AddonManagerSyncTests

open App.Core.Addons
open App.Core.Domain
open App.Core.State
open Moq
open NUnit.Framework

let mutable private addonManager = AddonManager()
let private iAddonHook () = (addonManager:>IAddonHook)
let private iAddonManager () = (addonManager:>IAddonManager)
let private appStateManager = Mock.Of<IAppStateManager>()

let mutable private beforeCounter1 = 0
let mutable private beforeCounter2 = 0
let mutable private afterStates1 = ResizeArray<IAppStateManager>()
let mutable private afterStates2 = ResizeArray<IAppStateManager>()
let mutable private tickState1 = (ref appStateManager, ResizeArray<Tick>())
let mutable private tickState2 = (ref appStateManager, ResizeArray<Tick>())

let private SetUpAddons (addonHook: IAddonHook) =
    let before1 () = beforeCounter1 <- beforeCounter1 + 1
    let before2 () = beforeCounter2 <- beforeCounter2 + 1
    let after1 state = afterStates1.Add(state)
    let after2 state = afterStates2.Add(state)
    let tick1 (tick, stateManager) =
        (fst tickState1) := stateManager
        (snd tickState1).Add(tick)
    let tick2 (tick, stateManager) =
        (fst tickState2) := stateManager
        (snd tickState2).Add(tick)
    let addon1 = {
        Id = "Uno"
        BeforeInitializeTask = before1 |> AddonSync |> Some
        AfterInitializeTask = after1 |> AddonSync |> Some
        TickTask = {ExecuteEvery = 2u |> Ticks; Task = AddonSync tick1} |> Some
    }
    let addon2 = {
        Id = "Due"
        BeforeInitializeTask = before2 |> AddonSync |> Some
        AfterInitializeTask = after2 |> AddonSync |> Some
        TickTask = {ExecuteEvery = 4u |> Ticks; Task = AddonSync tick2} |> Some
    }
    addonHook.RegisterAddon addon1
    addonHook.RegisterAddon addon2

[<SetUp>]
let public SetUp () = addonManager <- AddonManager()

[<Test>]
let public ``Before Initialize Tasks Are Called Once`` () =
    SetUpAddons (iAddonHook())
    let manager = iAddonManager()
    Assert.That(beforeCounter1, Is.EqualTo(0))
    Assert.That(beforeCounter2, Is.EqualTo(0))
    manager.CallBeforeInitialize()
    manager.CallBeforeInitialize()
    Assert.That(beforeCounter1, Is.EqualTo(1))
    Assert.That(beforeCounter2, Is.EqualTo(1))

[<Test>]
let public ``After Initialize Tasks Are Called Once`` () =
    SetUpAddons (iAddonHook())
    let manager = iAddonManager()
    Assert.That(afterStates1.Count, Is.EqualTo(0))
    Assert.That(afterStates2.Count, Is.EqualTo(0))
    manager.CallAfterInitialize appStateManager
    manager.CallAfterInitialize appStateManager
    Assert.That(afterStates1.Count, Is.EqualTo(1))
    Assert.That(afterStates2.Count, Is.EqualTo(1))

[<Test>]
let public ``After Initialize Tasks Are Called With Correct State`` () =
    SetUpAddons (iAddonHook())
    let manager = iAddonManager()
    manager.CallAfterInitialize appStateManager
    Assert.That(afterStates1 |> Seq.forall (fun state -> state = appStateManager))
    Assert.That(afterStates2 |> Seq.forall (fun state -> state = appStateManager))

[<Test>]
let public ``Tick Events Are Called With Correct Manager And Ticks`` () =
    SetUpAddons (iAddonHook())
    let manager = iAddonManager()

    for n in 0UL..10UL do
        manager.Update {TimeStamp = n} appStateManager

    Assert.That(!(fst tickState1), Is.EqualTo(appStateManager))
    Assert.That(!(fst tickState2), Is.EqualTo(appStateManager))

    Assert.That((snd tickState1).Count, Is.EqualTo(6))
    Assert.That((snd tickState2).Count, Is.EqualTo(3))

    Assert.That((snd tickState1).[0], Is.EqualTo({TimeStamp = 0UL}))
    Assert.That((snd tickState1).[5], Is.EqualTo({TimeStamp = 10UL}))
    Assert.That((snd tickState2).[0], Is.EqualTo({TimeStamp = 0UL}))
    Assert.That((snd tickState2).[2], Is.EqualTo({TimeStamp = 8UL}))
