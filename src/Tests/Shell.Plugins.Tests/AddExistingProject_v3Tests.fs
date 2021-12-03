module public App.Shell.Plugins.Tests.AddExistingProject_v3Tests

open System.IO
open App.Core.Domain
open App.Shell.Plugins
open FSharpPlus
open NUnit.Framework

let sut = AddExistingProject_v3.plugin

[<Test>]
let public ``Valid Project File Is SuccessFully Converted To Domain Project Object`` () =
    let path = Path.Combine("TestData", "TestProject", "project.godot")
    let projectFile = FileData.tryFind path |> unwrap |> ProjectFile
    match sut projectFile with
    | Ok p ->
        Assert.That(p.File, Is.EqualTo(projectFile))
        Assert.That(p.Name.Val, Is.EqualTo("9Whim-9")) // As defined in the test project file
        Assert.That(p.Path.Val.Name, Is.EqualTo("TestProject"))
        Assert.That(p.AssociatedEngine, Is.EqualTo(None))
    | Error err -> Assert.Fail($"Expected successful adding of project, but failed because: {err}")

[<Test>]
let public ``Project File That Does Not Exist Results In Error`` () =
    let pathToNothing = Path.Combine("TestData", "TestProject", "nothing.godot")
    File.Create pathToNothing |> ignore
    let projectFile = FileData.tryFind pathToNothing |> unwrap |> ProjectFile
    File.Delete pathToNothing
    match sut projectFile with
    | Ok p -> Assert.Fail($"Expected failure, but successfully added project {p}")
    | Error err -> Assert.Pass($"Failed correctly with error: \"{err}\"")

[<Test>]
let public ``Empty Project File Results In Error`` () =
    let path = Path.Combine("TestData", "TestProject", "empty.godot")
    let projectFile = FileData.tryFind path |> unwrap |> ProjectFile
    match sut projectFile with
    | Ok p -> Assert.Fail($"Expected failure, but successfully added project {p}")
    | Error err -> Assert.Pass($"Failed correctly with error: \"{err}\"")
