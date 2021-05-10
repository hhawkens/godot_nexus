module public App.Core.Addons.Tests.AddonControllerAsyncTests

open App.Core.Addons
open App.Core.Domain
open App.Core.State
open Moq
open NUnit.Framework

[<Literal>]
let sleepTime = 25
[<Literal>]
let sleepEpsilon = 5

let mutable private sut = AddonController()
let private iSutHook () = (sut:>IAddonHook)
let private iSutController () = (sut:>IAddonController)
let private appStateController = Mock.Of<IAppStateController>()
let sleep (ms: int) = System.Threading.Thread.Sleep ms
let last ls = Seq.reduce (fun _ -> id) ls

let mutable private beforeCounter1 = 0
let mutable private beforeCounter2 = 0
let mutable private afterStates1 = ResizeArray<IAppStateController>()
let mutable private afterStates2 = ResizeArray<IAppStateController>()
let mutable private tickState1 = (ref appStateController, ResizeArray<Tick>())
let mutable private tickState2 = (ref appStateController, ResizeArray<Tick>())

let private SetUpAddons (addonHook: IAddonHook) =
    let before1 () = async {
        sleep sleepTime
        beforeCounter1 <- beforeCounter1 + 1
    }
    let before2 () = async {
        sleep (sleepTime * 2)
        beforeCounter2 <- beforeCounter2 + 1
    }
    let after1 state = async {
        sleep sleepTime
        afterStates1.Add(state)
    }
    let after2 state = async {
        sleep (sleepTime * 2)
        afterStates2.Add(state)
    }
    let tick1 (tick, stateController) = async {
        sleep sleepTime
        (fst tickState1) := stateController
        (snd tickState1).Add(tick)
    }
    let tick2 (tick, stateController) = async {
        sleep (sleepTime * 2)
        (fst tickState2) := stateController
        (snd tickState2).Add(tick)
    }
    let addon1 = {
        Id = "Uno"
        BeforeInitializeTask = before1 |> AddonAsync |> Some
        AfterInitializeTask = after1 |> AddonAsync |> Some
        TickTask = {ExecuteEvery = 2u |> Ticks; Task = AddonAsync tick1} |> Some
    }
    let addon2 = {
        Id = "Due"
        BeforeInitializeTask = before2 |> AddonAsync |> Some
        AfterInitializeTask = after2 |> AddonAsync |> Some
        TickTask = {ExecuteEvery = 4u |> Ticks; Task = AddonAsync tick2} |> Some
    }
    addonHook.RegisterAddon addon1
    addonHook.RegisterAddon addon2

[<SetUp>]
let public SetUp () = sut <- AddonController()

[<Test>]
let public ``Before Initialize Tasks Are Called Once`` () =
    SetUpAddons (iSutHook())
    let controller = iSutController()
    controller.CallBeforeInitialize()
    Assert.That(beforeCounter1, Is.EqualTo(0))
    Assert.That(beforeCounter2, Is.EqualTo(0))
    sleep (sleepTime + sleepEpsilon)
    Assert.That(beforeCounter1, Is.EqualTo(1))
    Assert.That(beforeCounter2, Is.EqualTo(0))
    sleep (sleepTime + sleepEpsilon)
    Assert.That(beforeCounter1, Is.EqualTo(1))
    Assert.That(beforeCounter2, Is.EqualTo(1))
    controller.CallBeforeInitialize()
    sleep (sleepTime * 2 + sleepEpsilon)
    Assert.That(beforeCounter1, Is.EqualTo(1))
    Assert.That(beforeCounter2, Is.EqualTo(1))

[<Test>]
let public ``After Initialize Tasks Are Called Once`` () =
    SetUpAddons (iSutHook())
    let controller = iSutController()
    controller.CallAfterInitialize appStateController
    Assert.That(afterStates1.Count, Is.EqualTo(0))
    Assert.That(afterStates2.Count, Is.EqualTo(0))
    sleep (sleepTime + sleepEpsilon)
    Assert.That(afterStates1.Count, Is.EqualTo(1))
    Assert.That(afterStates2.Count, Is.EqualTo(0))
    sleep (sleepTime + sleepEpsilon)
    Assert.That(afterStates1.Count, Is.EqualTo(1))
    Assert.That(afterStates2.Count, Is.EqualTo(1))
    controller.CallAfterInitialize appStateController
    sleep (sleepTime * 2 + sleepEpsilon)
    Assert.That(afterStates1.Count, Is.EqualTo(1))
    Assert.That(afterStates2.Count, Is.EqualTo(1))

[<Test>]
let public ``After Initialize Tasks Are Called With Correct State`` () =
    SetUpAddons (iSutHook())
    let controller = iSutController()
    controller.CallAfterInitialize appStateController
    sleep (sleepTime * 2 + sleepEpsilon)
    Assert.That(afterStates1 |> Seq.forall (fun state -> state = appStateController))
    Assert.That(afterStates2 |> Seq.forall (fun state -> state = appStateController))

[<Test>]
let public ``Tick Events Are Called With Correct controller And Ticks`` () =
    SetUpAddons (iSutHook())
    let controller = iSutController()

    for n in 0UL..10UL do
        sleep (sleepTime + sleepEpsilon)
        controller.Update {TimeStamp = n} appStateController

    Assert.That(!(fst tickState1), Is.EqualTo(appStateController))
    Assert.That(!(fst tickState2), Is.EqualTo(appStateController))

    Assert.That((snd tickState1).Count, Is.LessThan(6))
    Assert.That((snd tickState2).Count, Is.LessThan(4))

    Assert.That((snd tickState1).[0], Is.EqualTo({TimeStamp = 0UL}))
    Assert.That(((snd tickState1) |> last).TimeStamp, Is.LessThan(10))
    Assert.That((snd tickState2).[0], Is.EqualTo({TimeStamp = 0UL}))
    Assert.That(((snd tickState2) |> last).TimeStamp, Is.LessThan(8))
