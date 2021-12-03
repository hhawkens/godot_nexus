module public App.Shell.Plugins.Tests.OpenProjectTests

open System
open System.IO
open FSharpPlus
open App.Core.Domain
open App.Shell.Plugins
open NUnit.Framework

let private sut = OpenProject.plugin
let private testDir = TestContext.CurrentContext.TestDirectory
let private tryDeleteDir dirData = DirectoryData.tryDelete dirData |> Result.mapError (fun _ -> "")

let private getEngine isValid id =
    let engineData = {Version = Version(1, 0); DotNetSupport = NoSupport}
    let dir = Directory.CreateDirectory $"engine-d-{id}"
    let file = File.Create $"engine-f-{id}"
    let engineInstall =
        EngineInstall.New
            engineData
            (dir.FullName |> DirectoryData.tryCreate |> unwrap)
            (file.Name |> FileData.tryCreate |> unwrap)
    if not isValid then do
        dir.Delete()
        File.Delete file.Name
    engineInstall

let private getProject isValid id =
    let name = "Test-P" |> ProjectName
    let dir = Directory.CreateDirectory $"project-d-{id}"
    let file = File.Create $"project-f-{id}"
    let project = {
        Name = name
        Path = dir.FullName |> DirectoryData.tryCreate |> unwrap |> ProjectDirectory
        File = file.Name |> FileData.tryCreate |> unwrap |> ProjectFile
        AssociatedEngine = None
    }
    if not isValid then do
        dir.Delete()
        File.Delete file.Name
    project

let private AssertFailure engineIsValid projectIsValid id =
    let engine = getEngine engineIsValid id
    let project = getProject projectIsValid id
    match sut engine project with
    | Ok _ -> Assert.Fail("Expected failure when opening project with invalid parameters, but succeeded!")
    | Error err ->
        Assert.That(
            err,
            Does.EndWith("Project file no longer exists!").Or
                .EndsWith(" not found!").Or
                .EndsWith(" nor project file!"))
        Assert.Pass($"Failed correctly with message:\n{err}")

let private tearDown id =
    $"engine-f-{id}" |> FileData.tryCreate >>= FileData.tryDelete |> ignore
    $"project-f-{id}" |> FileData.tryCreate >>= FileData.tryDelete |> ignore
    $"engine-d-{id}" |> DirectoryData.tryCreate >>= tryDeleteDir |> ignore
    $"project-d-{id}" |> DirectoryData.tryCreate >>= tryDeleteDir |> ignore

[<TestCase(true, false)>]
[<TestCase(false, true)>]
[<TestCase(false, false)>]
let public ``Invalid Engine Or Invalid Project Yields Error``engineIsValid projectIsValid =
    let id = (engineIsValid, projectIsValid)
    AssertFailure engineIsValid projectIsValid id
    tearDown id
