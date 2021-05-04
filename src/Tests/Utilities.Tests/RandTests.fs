module public App.Utilities.Tests.RandTests

open App.Utilities
open NUnit.Framework

[<Test>]
let public ``Next Integer Produces Unique Signed 32 Bit Numbers`` () =
    let numbers = seq { for _ in 1 .. 10000 -> Rand.NextI32() } |> Set
    Assert.That(numbers.Count, Is.EqualTo(10000))
    Assert.That(numbers.MinimumElement, Is.InstanceOf<int32>())

[<Test>]
let public ``Next Ulong Produces Unique Unsigned 64 Bit Numbers`` () =
    let numbers = seq { for _ in 1 .. 10000 -> Rand.NextU64() } |> Set
    Assert.That(numbers.Count, Is.EqualTo(10000))
    Assert.That(numbers.MinimumElement, Is.InstanceOf<uint64>())
