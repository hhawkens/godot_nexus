module public App.Utilities.Tests.FileSizeTests

open App.Utilities
open NUnit.Framework

[<Test>]
let public ``File Size Has Correct Kilobytes`` () =
    let fileSize = FileSize.FromKilobytes 19.55
    Assert.That(fileSize.Kilobytes, Is.EqualTo(19.55))
    Assert.That(fileSize.bytes, Is.EqualTo(19_550UL))

[<Test>]
let public ``File Size Has Correct Megabytes`` () =
    let fileSize = FileSize.FromMegabytes 19.55
    Assert.That(fileSize.Megabytes, Is.EqualTo(19.55))
    Assert.That(fileSize.bytes, Is.EqualTo(19_550_000UL))

[<Test>]
let public ``File Size Has Correct Gigabytes`` () =
    let fileSize = FileSize.FromGigabytes 19.55
    Assert.That(fileSize.Gigabytes, Is.EqualTo(19.55))
    Assert.That(fileSize.bytes, Is.EqualTo(19_550_000_000UL))

[<Test>]
let public ``File Size Has Correct Terabytes`` () =
    let fileSize = FileSize.FromTerabytes 19.55
    Assert.That(fileSize.Terabytes, Is.EqualTo(19.55))
    Assert.That(fileSize.bytes, Is.EqualTo(19_550_000_000_000UL))

[<Test>]
let public ``File Size Has Correct Petabytes`` () =
    let fileSize = FileSize.FromPetabytes 19.55
    Assert.That(fileSize.Petabytes, Is.EqualTo(19.55))
    Assert.That(fileSize.bytes, Is.EqualTo(19_550_000_000_000_000UL))

[<TestCase("")>]
[<TestCase(" ")>]
[<TestCase(" 100 ")>]
[<TestCase("a2mb")>]
[<TestCase("33GG")>]
[<TestCase("12mg")>]
let public ``Invalid Text Cannot Be Parsed`` txt =
    Assert.That(FileSize.FromText txt, Is.EqualTo(None))

[<TestCase("1.25Mb", 1_250_000UL)>]
[<TestCase("12K", 12_000UL)>]
[<TestCase("0.44 Gigabytes", 440_000_000UL)>]
[<TestCase("1  TB", 1_000_000_000_000UL)>]
[<TestCase("0.00000055  pB", 550_000_000UL)>]
[<TestCase("39 bytes", 39UL)>]
let public ``Valid Text Can Be Parsed`` txt bytes =
    Assert.That(FileSize.FromText txt, Is.EqualTo(Some {bytes = bytes}))
