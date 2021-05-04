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
