module public FSharpPlus.Tests.CompareTests

open FSharpPlus
open NUnit.Framework

type public TestType = {Name: string; Age: int}

[<Test>]
let public ``Diff Between Two Equal Objects Is Zero`` () =
    let t1 = {Name = "One"; Age = 1}
    let t2 = {Name = "One"; Age = 1}
    let diffs = Compare.allPropertyDiffs t1 t2
    Assert.That(diffs.Length, Is.EqualTo(0))

[<Test>]
let public ``Diff Shows All Differences`` () =
    let t1 = {Name = "One"; Age = 1}
    let t2 = {Name = "Two"; Age = 2}
    let diffs = Compare.allPropertyDiffs t1 t2
    Assert.That(diffs.Length, Is.EqualTo(2))
    Assert.That(diffs.[0].Name, Is.EqualTo("Name"))
    Assert.That(diffs.[1].Name, Is.EqualTo("Age"))

[<Test>]
let public ``Diff Shows One Differences`` () =
    let t1 = {Name = "One"; Age = 1}
    let t2 = {Name = "One"; Age = 2}
    let diffs = Compare.allPropertyDiffs t1 t2
    Assert.That(diffs.Length, Is.EqualTo(1))
    Assert.That(diffs.[0].Name, Is.EqualTo("Age"))
