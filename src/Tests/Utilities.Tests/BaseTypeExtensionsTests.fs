module public FSharpPlus.Tests.BaseTypeExtensionsTests

open NUnit.Framework
open FSharpPlus

[<TestCase("", "")>]
[<TestCase("ShowMeTHEMoney", "Show Me THE Money")>]
[<TestCase("AllRise", "All Rise")>]
[<TestCase("cuiBono", "cui Bono")>]
[<TestCase("AllXYZ", "All XYZ")>]
[<TestCase("XYZAll", "XYZ All")>]
let public ``Pascal Case Is Correctly Being Split By Whitespace`` (input: string) (output: string) =
    Assert.That(input |> SplitPascalCase, Is.EqualTo(output))
