module public App.Utilities.Tests.WorkFlowsTests

open NUnit.Framework
open App.Utilities

[<Test>]
let public ``Result Return From`` () =
    let res = result { return! Ok 1 }
    Assert.That(res, Is.EqualTo(Ok 1))

[<Test>]
let public ``Result Return`` () =
    let res = result { return "1" }
    Assert.That(res, Is.EqualTo(Ok "1"))

[<Test>]
let public ``Result Combine Return First`` () =
    let res = result {
        return! Ok 11
        return! Ok 22
    }
    Assert.That(res, Is.EqualTo(Ok 11))

[<Test>]
let public ``Result Combine Returns Second`` () =
    let res = result {
        return! Error ""
        return! Ok 22
    }
    let expected = Result<int, string>.Ok 22
    Assert.That(res, Is.EqualTo(expected))

[<Test>]
let public ``Result Combine Returns First`` () =
    let res = result {
        return! Error "one"
        return! Error "two"
    }
    Assert.That(res, Is.EqualTo(Error "two"))

[<Test>]
let public ``Result Combined Return And ReturnFrom`` () =
    let res = result {
        return! Error ""
        return 66
    }
    let expected = Result<int, string>.Ok 66
    Assert.That(res, Is.EqualTo(expected))

[<Test>]
let public ``Result Bind Handles Some Correctly`` () =
    let res = result {
        let! x = Ok 44
        let! y = Ok 11
        return x + y
    }
    Assert.That(res, Is.EqualTo(Ok 55))

[<Test>]
let public ``Result Bind Handles None Correctly`` () =
    let res = result {
        let! x = Error "uno"
        let! y = Ok 100
        return x + y
    }
    let expected = Result<int, string>.Error "uno"
    Assert.That(res, Is.EqualTo(expected))
