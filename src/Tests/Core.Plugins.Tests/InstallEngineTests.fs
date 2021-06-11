module public App.Core.Plugins.Tests.InstallEngineTests

open System
open System.IO
open FSharpPlus
open App.Core.Domain
open App.Utilities
open NUnit.Framework
open App.Core.Plugins

let private testEngineDirectory = "TestEngineDirectory" |> DirectoryData.tryCreate |> unwrap |> EnginesDirectory
let private testZipFile = "TestData/Godotv3.zip" |> FileData.tryFind |> unwrap |> EngineZipFile
let private testEngineData = {Version = Version(3,0,0); DotNetSupport = NoSupport}

[<SetUp>]
let public SetUp() = "TestEngineDirectory" |> DirectoryData.tryCreate |> unwrap |> ignore

[<TearDown>]
let public TearDown () = testEngineDirectory.Val.TryDelete() |> ignore

[<Test>]
let public ``Engine Is Correctly Installed If All Files Are Intact`` () =
    let sut = InstallEngine.plugin testZipFile testEngineDirectory testEngineData

    sut.Run () |> Async.RunSynchronously

    match sut.EndStatus with
    | Succeeded engineInstall ->
        let expectedEnginePath = Path.Combine(testEngineDirectory.Val.FullPath, "3.0.0")
        let expectedEngineFile = Path.Combine (expectedEnginePath, "Godotv3", "bin", "Godot.bin")
        Assert.That(engineInstall.Directory.StillExists)
        Assert.That(engineInstall.Directory.FullPath, Is.EqualTo(expectedEnginePath))
        Assert.That(engineInstall.Data.Version, Is.EqualTo(Version(3,0,0)))
        Assert.That(engineInstall.Data.DotNetSupport, Is.EqualTo(NoSupport))
        Assert.That(engineInstall.ExecutableFile.StillExists)
        Assert.That(engineInstall.ExecutableFile.FullPath, Is.EqualTo(expectedEngineFile))
    | _ ->
        Assert.Fail("Expected installation job to succeed!")

[<Test>]
let public ``Engine Installation Fails If There Is No Zip File`` () =
    File.Create("TestData/Godotv0.zip") |> dispose
    let zipFile = "TestData/Godotv0.zip" |> FileData.tryFind |> unwrap |> EngineZipFile
    File.Delete("TestData/Godotv0.zip")
    let sut = InstallEngine.plugin zipFile testEngineDirectory testEngineData

    sut.Run () |> Async.RunSynchronously

    match sut.EndStatus with
    | Failed msg -> Assert.Pass($"Installation failed as expected with message: {msg}")
    | _ -> Assert.Fail("Expected installation to fail!")

[<Test>]
let public ``Engine Installation Fails If There Is No EngineDirectory`` () =
    testEngineDirectory.Val.TryDelete() |> ignore
    let sut = InstallEngine.plugin testZipFile testEngineDirectory testEngineData

    sut.Run () |> Async.RunSynchronously

    match sut.EndStatus with
    | Failed msg -> Assert.Pass($"Installation failed as expected with message: {msg}")
    | _ -> Assert.Fail("Expected installation to fail!")

[<Test>]
let public ``Engine Installation Fails If There Is No Executable`` () =
    let zipFile = "TestData/GodotX.zip" |> FileData.tryFind |> unwrap |> EngineZipFile
    let sut = InstallEngine.plugin zipFile testEngineDirectory testEngineData

    sut.Run () |> Async.RunSynchronously

    match sut.EndStatus with
    | Failed msg -> Assert.Pass($"Installation failed as expected with message: {msg}")
    | _ -> Assert.Fail("Expected installation to fail!")
