module public App.Utilities.Tests.DirectoryDataTests

open System.IO
open FSharpPlus
open NUnit.Framework
open App.Utilities
open App.TestHelpers

[<Literal>]
let private TestFolder = "TestData"

[<Literal>]
let private TestSubFolder = "TestData/Sub"

[<Literal>]
let private NewFolder = "NewFolder"

let private RemoveNewFolder() =
    match Directory.Exists NewFolder with
        | true -> Directory.Delete NewFolder
        | false -> ()

[<SetUp>]
let public SetUp() = RemoveNewFolder()

[<TearDown>]
let public TearDown() = RemoveNewFolder()

[<Test>]
let public ``TryFind Finds Existing Directory`` () =
    match DirectoryData.tryFind TestFolder with
        | Some folder -> Assert.That(folder.FullPath, Does.EndWith($"/{TestFolder}"))
        | None -> Assert.Fail("Could not find test folder")

[<Test>]
let public ``TryFind Does Not Find Missing Directory`` () =
    match DirectoryData.tryFind "NotThere" with
        | Some f -> Assert.Fail($"Found a folder that is not supposed to be there: {f.FullPath}")
        | None -> Assert.Pass()

[<TestCase "">]
let public ``TryCreate With Invalid Input Fails`` input =
    match DirectoryData.tryCreate input with
        | Ok f -> Assert.Fail($"Folder {f.FullPath} was not supposed to be created")
        | Error _ -> Assert.Pass()

[<Test>]
let public ``TryCreate With Valid Input Succeeds`` () =
    Assert.That(Directory.Exists(NewFolder), Is.False)
    match DirectoryData.tryCreate NewFolder with
        | Ok f ->
            Assert.That(f.FullPath, Does.EndWith($"/{NewFolder}"))
            Assert.That(f.Name, Is.EqualTo(NewFolder))
        | Error err -> Assert.Fail($"Could not create test folder, because {err}")
    Assert.That(Directory.Exists(NewFolder), Is.True)

[<Test>]
let public ``StillExists Reports Accurate Status`` () =
    match DirectoryData.tryCreate NewFolder with
        | Ok folder ->
            Assert.That(folder.StillExists, Is.True)
            RemoveNewFolder()
            Assert.That(folder.StillExists, Is.False)
        | Error _ -> Assert.Fail()

[<Test>]
let public ``Files Shows All Available Files`` () =
    match DirectoryData.tryFind TestFolder with
        | Some folder ->
            let files = folder.Files
            Assert.That(files.Length, Is.EqualTo(3))
            Assert.That(files |> exists (fun f -> f.Name = "Test.txt"))
            Assert.That(files |> exists (fun f -> f.Name = "Test_1.txt"))
            Assert.That(files |> exists (fun f -> f.Name = "Test_2.txt"))
        | None -> Assert.Fail("Could not find test folder")

[<Test>]
let public ``SubDirectories Shows All Available Sub Directories`` () =
    match DirectoryData.tryFind TestFolder with
        | Some folder ->
            let dirs = folder.SubDirectories
            Assert.That(dirs.Length, Is.EqualTo(1))
            Assert.That(dirs.[0].Name, Is.EqualTo("Sub"))
        | None -> Assert.Fail("Could not find test folder")

[<Test>]
let public ``Parent Gets Parent Folder`` () =
    match DirectoryData.tryFind TestSubFolder with
        | Some folder ->
            match folder.Parent with
            | Ok parent -> Assert.That(parent.Name, Is.EqualTo("TestData"))
            | Error err -> Assert.Fail(err)
        | None -> Assert.Fail("Could not find test folder")

[<Test>]
let public ``MsBuildSolutionFolder Finds The Correct Folder`` () =
    Assert.That(msBuildSolutionFolder.FullPath, Is.Not.Null)
    Assert.That(msBuildSolutionFolder.FullPath, Is.Not.Empty)
    Assert.That(msBuildSolutionFolder.FullPath.EndsWith("src"))

[<Test>]
let public ``Find Files With Predicate Finds The Correct Files`` () =
    let sut = DirectoryData.tryFind TestFolder |> unwrap
    let files = sut.FindFilesRecWhere (fun f -> f.Name.StartsWith("Test_"))
    Assert.That(files.Length, Is.EqualTo(3)) // Making sure Test.txt is not among the files
    Assert.That(files |> exists (fun f -> f.Name = "Test_1.txt"))
    Assert.That(files |> exists (fun f -> f.Name = "Test_2.txt"))
    Assert.That(files |> exists (fun f -> f.Name = "Test_3.txt"))

[<Test>]
let public ``Finds Correct Directory Of Given File`` () =
    let testFile = FileData.tryFind $"{TestSubFolder}/Test_3.txt" |> unwrap
    let sut = DirectoryData.from testFile
    Assert.That(sut.FullPath, Does.EndWith($"{TestSubFolder}"))

[<Test>]
let public ``Trying To Delete Existing Directory Succeeds`` () =
    let sut = DirectoryData.tryCreate "ToBeDeleted" |> unwrap
    Assert.That(sut.StillExists)
    let deletionResult = sut.TryDelete()
    Assert.That(deletionResult.IsOk)
    Assert.That(sut.StillExists, Is.False)

[<Test>]
let public ``Trying To Delete Existing Directory With Files And SubDirs Succeeds`` () =
    let sut = DirectoryData.tryCreate "ToBeDeleted" |> unwrap
    Directory.CreateDirectory("ToBeDeleted/Sub") |> ignore
    File.Create("ToBeDeleted/F1.bin") |> dispose
    File.Create("ToBeDeleted/F2.bin") |> dispose
    File.Create("ToBeDeleted/Sub/F3.txt") |> dispose

    Assert.That(sut.StillExists)
    let deletionResult = sut.TryDelete()
    Assert.That(deletionResult.IsOk)
    Assert.That(sut.StillExists, Is.False)

[<Test>]
let public ``Trying To Delete Not Existing Directory Fails`` () =
    let sut = DirectoryData.tryCreate "ToBeDeleted" |> unwrap
    Directory.Delete sut.FullPath
    Assert.That(sut.StillExists, Is.False)

    let deletionResult = sut.TryDelete()
    Assert.That(deletionResult.IsError)
    Assert.That(sut.StillExists, Is.False)
