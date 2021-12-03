module public App.Shell.Plugins.Tests.RemoveProjectTests

open System.IO
open App.Shell.Plugins
open App.Core.Domain
open App.Core.PluginDefinitions
open FSharpPlus
open NUnit.Framework

[<Literal>]
let private TestProjectFolder = "ProjectFolderToRemove"

[<Literal>]
let private TestProjectFile = "project.godot"

let private pathToProjectFile = Path.Combine(TestProjectFolder, TestProjectFile)

let private testProject () = {
    Name = "Irrelevant" |> ProjectName
    Path = DirectoryData.tryCreate TestProjectFolder |> unwrap |> ProjectDirectory
    File = FileData.tryCreate pathToProjectFile |> unwrap |> ProjectFile
    AssociatedEngine = None
}

let private sut = RemoveProject.plugin

[<SetUp>]
let public setUp () =
    Directory.CreateDirectory TestProjectFolder |> ignore
    File.Create pathToProjectFile |> ignore

[<TearDown>]
let public tearDown () =
    match DirectoryData.tryCreate TestProjectFolder with
    | Ok dd -> dd.TryDelete () |> ignore
    | _ -> ()

[<Test>]
let public ``Existing Project Is Removed Successfully`` () =
    let testProject = testProject ()
    match sut testProject with
    | SuccessfulRemoval -> ()
    | err -> Assert.Fail($"Expected successful removal, but was: {err}")

    Assert.That(Directory.Exists(TestProjectFolder), Is.False)
    Assert.That(File.Exists(pathToProjectFile), Is.False)

[<Test>]
let public ``Missing Project Is Reported Missing`` () =
    let testProject = testProject ()
    tearDown ()
    Assert.That(Directory.Exists(TestProjectFolder), Is.False)
    Assert.That(File.Exists(pathToProjectFile), Is.False)

    match sut testProject with
    | NotFound -> ()
    | other -> Assert.Fail($"Expected project-folder-not-found, but was: {other}")
