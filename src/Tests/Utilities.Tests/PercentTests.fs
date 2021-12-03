module public FSharpPlus.Tests.PercentTests

open FSharpPlus
open NUnit.Framework

[<Literal>]
let private ValidPercentFailedMessage = "Valid percent number should not be none"
[<Literal>]
let private InvalidInputMessage = "Invalid percent input should not be accepted"

[<TestCase(0uy)>]
[<TestCase(100uy)>]
[<TestCase(1uy)>]
[<TestCase(99uy)>]
[<TestCase(55uy)>]
let public ``Percent Object Successfully Created With Valid Int`` (validInt) =
    let validPercent = Percent.FromInt validInt
    let expected = float validInt / 100.0
    match validPercent with
    | Some p -> Assert.That(p.Value, Is.EqualTo(expected))
    | None -> Assert.Fail(ValidPercentFailedMessage)

[<TestCase(101uy)>]
[<TestCase(199uy)>]
[<TestCase(255uy)>]
let public ``Percent Object Not Created With Invalid Int`` (invalidInt) =
    let invalidPercent = Percent.FromInt invalidInt
    match invalidPercent with
    | Some _ -> Assert.Fail(InvalidInputMessage)
    | None -> Assert.Pass()

[<TestCase(0.0)>]
[<TestCase(0.000001)>]
[<TestCase(0.999999)>]
[<TestCase(1.0)>]
let public ``Percent Object Successfully Created With Valid Float`` (validFloat) =
    let validPercent = Percent.FromFloat validFloat
    match validPercent with
    | Some _ -> Assert.Pass()
    | None -> Assert.Fail(ValidPercentFailedMessage)

[<TestCase(0.0)>]
[<TestCase(0.999)>]
[<TestCase(0.346)>]
[<TestCase(0.894)>]
let public ``Percent Object Created With Valid Float Has Correct Value`` input =
    let percent = Percent.FromFloat input
    match percent with
        | Some p -> Assert.That(p.Value, Is.EqualTo(input))
        | None -> Assert.Fail(ValidPercentFailedMessage)

[<TestCase(-0.00001)>]
[<TestCase(1.000001)>]
[<TestCase(-22.0)>]
[<TestCase(1234.2342)>]
let public ``Percent Object Not Created With Invalid Float`` (invalidFloat) =
    let invalidPercent = Percent.FromFloat invalidFloat
    match invalidPercent with
    | Some _ -> Assert.Fail(InvalidInputMessage)
    | None -> Assert.Pass()

[<Test>]
let public ``ToString Shows Formatted Percent Value`` () =
    let percent = (Percent.FromFloat 0.4478).Value
    Assert.That(percent.ToString(), Is.EqualTo("44.8 %"))
