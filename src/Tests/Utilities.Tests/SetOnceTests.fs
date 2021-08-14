module public App.Utilities.Tests.SetOnceTests

open System
open App.Utilities
open NUnit.Framework

[<Literal>]
let private TestText = "Test"
let private newSut () = SetOnce(TestText)

[<Test>]
let public ``Has Correct Initial Value`` () =
    let sut = newSut()
    Assert.That(sut.Value, Is.EqualTo(TestText))

[<Test>]
let public ``Other Value Can Be Set`` () =
    let sut = newSut()
    sut.Set("Other")
    Assert.That(sut.Value, Is.EqualTo("Other"))

[<Test>]
let public ``Other Value Can Be Set Checked`` () =
    let sut = newSut()
    let wasSet = sut.SetChecked("Other")
    Assert.That(sut.Value, Is.EqualTo("Other"))
    Assert.That(wasSet, Is.EqualTo(WasSet))

[<Test>]
let public ``Other Value Can Be Set Without Fail`` () =
    let sut = newSut()
    sut.SetOrFail("Other")
    Assert.That(sut.Value, Is.EqualTo("Other"))

[<Test>]
let public ``Other Value Cannot Be Set Twice`` () =
    let sut = newSut()
    sut.Set("Other")
    sut.Set("Twice")
    sut.SetChecked("Thrice") |> ignore
    Assert.That(sut.Value, Is.EqualTo("Other"))

[<Test>]
let public ``Other Value Cannot Be Set Twice Checked`` () =
    let sut = newSut()
    sut.SetChecked("Other") |> ignore
    let wasSet = sut.SetChecked("Twice")
    sut.Set("Thrice")
    Assert.That(sut.Value, Is.EqualTo("Other"))
    Assert.That(wasSet, Is.EqualTo(NotSet))

[<Test>]
let public ``Other Value Cannot Be Set Twice Without Fail`` () =
    let sut = newSut()
    sut.SetOrFail("Other")
    Assert.That(Action (fun () -> sut.SetOrFail("Other")), Throws.Exception)

[<Test>]
let public ``Same Value Can Only Be Set Once`` () =
    let sut = newSut()
    let wasSet1 = sut.SetChecked(TestText)
    let wasSet2 = sut.SetChecked(TestText)
    Assert.That(sut.Value, Is.EqualTo(TestText))
    Assert.That(wasSet1, Is.EqualTo(WasSet))
    Assert.That(wasSet2, Is.EqualTo(NotSet))
