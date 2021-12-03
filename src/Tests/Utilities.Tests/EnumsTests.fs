module public FSharpPlus.Tests.EnumsTests

open FSharpPlus
open NUnit.Framework

type public TestEnum =
    | One = 1
    | Two = 2
    | Three = 3


[<SetUp>]
let public Setup () =
    ()

[<Test>]
let public ``Iterate Returns All Enum Values`` () =
    let allEnumValues = Enums.iterate<TestEnum>()
    Assert.That(allEnumValues.Length, Is.EqualTo(3))
    Assert.That(allEnumValues.[0], Is.EqualTo(TestEnum.One))
    Assert.That(allEnumValues.[1], Is.EqualTo(TestEnum.Two))
    Assert.That(allEnumValues.[2], Is.EqualTo(TestEnum.Three))

[<Test>]
let public ``Parse Converts Valid Values`` () =
    Assert.That(Enums.tryParse<TestEnum> "One", Is.EqualTo(Some TestEnum.One))
    Assert.That(Enums.tryParse<TestEnum> "two", Is.EqualTo(Some TestEnum.Two))
    Assert.That(Enums.tryParse<TestEnum> "threE", Is.EqualTo(Some TestEnum.Three))
    Assert.That(Enums.tryParse<TestEnum> "Four", Is.EqualTo(None))
