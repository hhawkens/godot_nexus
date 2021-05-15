module public App.Utilities.Tests.WorkFlowsTests

open NUnit.Framework
open App.Utilities

[<Test>]
let public ``Maybe Return From`` () =
    let opt = maybe { return! Some 1 }
    Assert.That(opt, Is.EqualTo(Some 1))

[<Test>]
let public ``Maybe Return`` () =
    let opt = maybe { return "1" }
    Assert.That(opt, Is.EqualTo(Some "1"))

[<Test>]
let public ``Maybe Combine Return First`` () =
    let opt = maybe {
        return! Some 11
        return! Some 22
    }
    Assert.That(opt, Is.EqualTo(Some 11))

[<Test>]
let public ``Maybe Combine Return Second`` () =
    let opt = maybe {
        return! None
        return! Some 22
    }
    Assert.That(opt, Is.EqualTo(Some 22))

[<Test>]
let public ``Maybe Combine Return None`` () =
    let opt = maybe {
        return! None
        return! None
    }
    Assert.That(opt, Is.EqualTo(None))

[<Test>]
let public ``Maybe Combine Return Combined`` () =
    let opt = maybe {
        return! None
        return 66
    }
    Assert.That(opt, Is.EqualTo(Some 66))

[<Test>]
let public ``Maybe Bind Handles Some Correctly`` () =
    let opt = maybe {
        let! x = Some 44
        let! y = Some 11
        return x + y
    }
    Assert.That(opt, Is.EqualTo(Some 55))

[<Test>]
let public ``Maybe Bind Handles None Correctly`` () =
    let opt = maybe {
        let! x = None
        let! y = Some 100
        return x + y
    }
    Assert.That(opt, Is.EqualTo(None))

// ---------------------------------------------------------------

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
