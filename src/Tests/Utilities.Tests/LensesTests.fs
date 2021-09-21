module public App.Utilities.Tests.LensesTests

open NUnit.Framework
open App.Utilities

type public One = { Val: string }
type public Two = { OneVal: One; OtherVal: int }
type public Three = { TwoVal: Two; NextVal: obj }

let public Un = { Val = "Initial" }
let public Dos = { OneVal = Un; OtherVal = -1 }
let public Tres = { TwoVal = Dos; NextVal = obj() }

let private getNumberOfChangesToRecord (r: Three) =
    let mutable changes = 0
    if r.NextVal <> obj() then do changes <- changes + 1
    if r.TwoVal.OtherVal <> -1 then do changes <- changes + 1
    if r.TwoVal.OneVal.Val <> "Initial" then do changes <- changes + 1
    changes


[<Test>]
let public ``Lenses Can Update Nested Record Value`` () =
    let updatedRecord = Tres |> withLens <@ Tres.TwoVal.OtherVal @> 999 |> unwrap

    Assert.That(getNumberOfChangesToRecord Tres, Is.EqualTo(1))
    Assert.That(updatedRecord.TwoVal.OtherVal, Is.EqualTo(999))

[<Test>]
let public ``Lenses Can Update Deeply Nested Record Value`` () =
    let updatedRecord = Tres |> withLens <@ Tres.TwoVal.OneVal.Val @> "New One" |> unwrap

    Assert.That(getNumberOfChangesToRecord Tres, Is.EqualTo(1))
    Assert.That(updatedRecord.TwoVal.OneVal.Val, Is.EqualTo("New One"))

[<Test>]
let public ``Nonsensical Input Fails`` () =
    let origin = "Origin"
    let updatedRecord = origin |> withLens <@ origin.Length @> 5

    match updatedRecord with
    | Error err -> Assert.Pass($"Correctly failed with error: {err}")
    | Ok _ -> Assert.Fail("Expected failure!")
