module public FSharpPlus.Tests.LensesTests

open NUnit.Framework
open FSharpPlus

type public One = { Val: string }
type public Two = { OneVal: One; OtherVal: int }
type public Three = { TwoVal: Two; NextVal: obj }

let public Un = { Val = "Initial" }
let public Dos = { OneVal = Un; OtherVal = -1 }
let public Tres = { TwoVal = Dos; NextVal = obj() }

let private getNumberOfChangesToRecord (r: Three) =
    let mutable changes = 0
    if r.NextVal <> obj() then do changes <- changes + 1
    if r.TwoVal.OtherVal <> -1 then do changes <- changes + 1
    if r.TwoVal.OneVal.Val <> "Initial" then do changes <- changes + 1
    changes


[<Test>]
let public ``Lenses Can Update Nested Record Value`` () =
    let updatedRecord = Tres |> lens <@ Tres.TwoVal.OtherVal @> 999 |> unwrap

    Assert.That(getNumberOfChangesToRecord Tres, Is.EqualTo(1))
    Assert.That(updatedRecord.TwoVal.OtherVal, Is.EqualTo(999))

[<Test>]
let public ``Lenses Can Update Deeply Nested Record Value`` () =
    let updatedRecord = Tres |> lens <@ Tres.TwoVal.OneVal.Val @> "New One" |> unwrap

    Assert.That(getNumberOfChangesToRecord Tres, Is.EqualTo(1))
    Assert.That(updatedRecord.TwoVal.OneVal.Val, Is.EqualTo("New One"))

[<Test>]
let public ``Nonsensical Input Fails`` () =
    let origin = "Origin"
    let updatedRecord = origin |> lens <@ origin.Length @> 5

    match updatedRecord with
    | Error err -> Assert.Pass($"Correctly failed with error: {err}")
    | Ok _ -> Assert.Fail("Expected failure!")

[<Test>]
let public ``More Complex Nested Records Are Updated Correctly`` () =
    let enginesPath = {Description = "Engines Path"; DefaultValue = "/Engines"; CurrentValue = "/Engines"}
    let projectsPath = {Description = "Projects Path"; DefaultValue = "/Proj"; CurrentValue = "/Proj"}
    let theme = {Description = "Theme"; DefaultValue = TestTheme.System; CurrentValue = TestTheme.System}
    let general = {EnginesPath = enginesPath; ProjectsPath = projectsPath}
    let ui = {Theme = theme}
    let prefs = {General = general; UI = ui}

    let prefsV2 = prefs |> lens <@ prefs.UI.Theme.CurrentValue @> TestTheme.Dark |> unwrap
    Assert.That(prefsV2.UI.Theme.CurrentValue, Is.EqualTo(TestTheme.Dark))

    let prefsV3 = prefsV2 |> lens <@ prefsV2.General.EnginesPath.DefaultValue @> "New Default" |> unwrap
    Assert.That(prefsV3.UI.Theme.CurrentValue, Is.EqualTo(TestTheme.Dark))
    Assert.That(prefsV3.General.EnginesPath.DefaultValue, Is.EqualTo("New Default"))

    let prefsV4 =
        prefsV3 |> lens <@ prefsV3.General.ProjectsPath @>
            {Description = "New Desc"; DefaultValue = "New Def"; CurrentValue = "New Curr"} |> unwrap

    Assert.That(prefsV4.UI.Theme.CurrentValue, Is.EqualTo(TestTheme.Dark))
    Assert.That(prefsV4.General.EnginesPath.DefaultValue, Is.EqualTo("New Default"))
    Assert.That(prefsV4.General.ProjectsPath.Description, Is.EqualTo("New Desc"))
    Assert.That(prefsV4.General.ProjectsPath.DefaultValue, Is.EqualTo("New Def"))
    Assert.That(prefsV4.General.ProjectsPath.CurrentValue, Is.EqualTo("New Curr"))
    // Unchanged values
    Assert.That(prefsV4.UI.Theme.Description, Is.EqualTo("Theme"))
    Assert.That(prefsV4.UI.Theme.DefaultValue, Is.EqualTo(TestTheme.System))
    Assert.That(prefsV4.General.EnginesPath.Description, Is.EqualTo("Engines Path"))
    Assert.That(prefsV4.General.EnginesPath.CurrentValue, Is.EqualTo("/Engines"))
