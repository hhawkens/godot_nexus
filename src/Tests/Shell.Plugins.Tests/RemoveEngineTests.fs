module public App.Shell.Plugins.Tests.RemoveEngineTests

open System
open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Shell.Plugins
open FSharpPlus
open NUnit.Framework

[<Literal>]
let private testEngineDir = "Engine_v1"

let testEngineData = {Version = Version(1,0,0); DotNetSupport = NoSupport}
let sut = RemoveEngine.plugin

[<TearDown>]
let public TearDown () =
    if Directory.Exists testEngineDir then
        Directory.Delete testEngineDir

let private newEngineInstall () =
    Directory.CreateDirectory testEngineDir |> ignore
    Directory.CreateDirectory (Path.Combine(testEngineDir, "subFolder")) |> ignore
    File.Create (Path.Combine(testEngineDir, "subFolder", "Data.bin")) |> ignore
    EngineInstall.New
        testEngineData
        (testEngineDir |> DirectoryData.tryCreate |> unwrap)
        (Path.Combine(testEngineDir, "subFolder", "Data.bin") |> FileData.tryCreate |> unwrap)

[<Test>]
let public ``Existing Project Is Deleted Successfully`` () =
    let engineInstall = newEngineInstall ()
    let result = sut engineInstall
    match result with
    | SuccessfulRemoval -> Assert.Pass()
    | err -> Assert.Fail($"Expected removal success, but was error: {err}")

[<Test>]
let public ``Missing Project Folder Yields "EngineNotFound" Error`` () =
    let engineInstall = newEngineInstall ()
    testEngineDir |> DirectoryData.tryCreate |> unwrap |> DirectoryData.tryDelete |> ignore
    let result = sut engineInstall
    match result with
    | NotFound -> Assert.Pass()
    | other -> Assert.Fail($"Expected engine not found error, but was error: {other}")
