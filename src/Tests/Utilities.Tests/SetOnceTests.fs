module public App.Utilities.Tests.SetOnceTests

open System
open App.Utilities
open NUnit.Framework

[<Literal>]
let private TestText = "Test"
let private setOnce () = SetOnce(TestText)

[<Test>]
let public ``Has Correct Initial Value`` () =
    let once = setOnce()
    Assert.That(once.Value, Is.EqualTo(TestText))

[<Test>]
let public ``Other Value Can Be Set`` () =
    let once = setOnce()
    once.Set("Other")
    Assert.That(once.Value, Is.EqualTo("Other"))

[<Test>]
let public ``Other Value Can Be Set Checked`` () =
    let once = setOnce()
    let wasSet = once.SetChecked("Other")
    Assert.That(once.Value, Is.EqualTo("Other"))
    Assert.That(wasSet, Is.EqualTo(WasSet))

[<Test>]
let public ``Other Value Can Be Set Without Fail`` () =
    let once = setOnce()
    once.SetOrFail("Other")
    Assert.That(once.Value, Is.EqualTo("Other"))

[<Test>]
let public ``Other Value Cannot Be Set Twice`` () =
    let once = setOnce()
    once.Set("Other")
    once.Set("Twice")
    once.SetChecked("Thrice") |> ignore
    Assert.That(once.Value, Is.EqualTo("Other"))

[<Test>]
let public ``Other Value Cannot Be Set Twice Checked`` () =
    let once = setOnce()
    once.SetChecked("Other") |> ignore
    let wasSet = once.SetChecked("Twice")
    once.Set("Thrice")
    Assert.That(once.Value, Is.EqualTo("Other"))
    Assert.That(wasSet, Is.EqualTo(NotSet))

[<Test>]
let public ``Other Value Cannot Be Set Twice Without Fail`` () =
    let once = setOnce()
    once.SetOrFail("Other")
    Assert.That(Action (fun () -> once.SetOrFail("Other")), Throws.Exception)

[<Test>]
let public ``Same Value Can Only Be Set Once`` () =
    let once = setOnce()
    let wasSet1 = once.SetChecked(TestText)
    let wasSet2 = once.SetChecked(TestText)
    Assert.That(once.Value, Is.EqualTo(TestText))
    Assert.That(wasSet1, Is.EqualTo(WasSet))
    Assert.That(wasSet2, Is.EqualTo(NotSet))
