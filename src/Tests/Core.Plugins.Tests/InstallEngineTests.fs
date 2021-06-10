module public App.Core.Plugins.Tests.InstallEngineTests

open System
open System.IO
open App.Core.Domain
open App.Utilities
open NUnit.Framework
open App.Core.Plugins

let private testEngineDirectory = "TestEngineDirectory" |> DirectoryData.tryCreate |> unwrap |> EnginesDirectory

[<TearDown>]
let public TearDown () = testEngineDirectory.Val.TryDelete() |> ignore

[<Test>]
let public ``Engine Is Correctly Installed If All Files Are Intact`` () =
    let ziFile = "TestData/Godotv3.zip" |> FileData.tryFind |> unwrap |> EngineZipFile
    let version = Version(3,0,0)
    let engineData = {Version = version; DotNetSupport = NoSupport}
    let sut = InstallEngine.plugin ziFile testEngineDirectory engineData

    sut.Run () |> Async.RunSynchronously
    match sut.EndStatus with
    | Succeeded engineInstall ->
        let expectedEnginePath = Path.Combine(testEngineDirectory.Val.FullPath, "3.0.0")
        let expectedEngineFile = Path.Combine (expectedEnginePath, "Godotv3", "bin", "Godot.bin")
        Assert.That(engineInstall.Directory.StillExists)
        Assert.That(engineInstall.Directory.FullPath, Is.EqualTo(expectedEnginePath))
        Assert.That(engineInstall.Data.Version, Is.EqualTo(version))
        Assert.That(engineInstall.Data.DotNetSupport, Is.EqualTo(NoSupport))
        Assert.That(engineInstall.ExecutableFile.StillExists)
        Assert.That(engineInstall.ExecutableFile.FullPath, Is.EqualTo(expectedEngineFile))
    | _ ->
        Assert.Fail("Expected installation job to succeed!")
