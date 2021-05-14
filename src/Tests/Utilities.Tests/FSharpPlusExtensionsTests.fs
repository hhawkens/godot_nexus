module public App.Utilities.Tests.FSharpPlusExtensionsTests

open NUnit.Framework
open FSharpPlus

let public differenceTestCases = [|
    [|[]; []; []|]
    [|[1; 2]; []; [1; 2]|]
    [|[]; [1; 2]; []|]
    [|[1; 2; 3; 4; 10]; [1; 2; 9; 10]; [3; 4]|]
|]

[<TestCaseSource(nameof differenceTestCases)>]
let public ``Difference Returns The Expected Subset of Elements`` (a, b, expected) =
    Assert.That(difference (fun x y -> x = y) a b, Is.EqualTo(expected))

[<Test>]
let public ``Difference Uses Equality Check Correctly`` () =
    let a = [1; 3; 5; 7; 9]
    let b = ["1"; "2"; "3"; "4"; "5"]
    let equalityCheck x y = x.ToString() = y
    let expectedResult = [7; 9]
    Assert.That(difference equalityCheck a b, Is.EqualTo(expectedResult))
