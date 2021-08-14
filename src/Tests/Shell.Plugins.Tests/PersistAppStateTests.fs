module public App.Shell.Plugins.Tests.PersistAppStateTests

open System
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Shell.Plugins
open App.Utilities
open NUnit.Framework

let private newPlugin () =
    let plugin = PersistAppState():>UPersistAppState
    plugin.TryInitialize() |> unwrap
    plugin

let godotFile = FileData.tryCreate "TestData/Godot.bin" |> unwrap
let activeEngineInstall =
    EngineInstall.New {Version = Version(7,7,7); DotNetSupport = Mono} (DirectoryData.current()) godotFile

let private testAppState = {
    EnginesOnline = [
        EngineOnline.New {Version = Version(1,2,3); DotNetSupport = NoSupport} "url" (FileSize.FromMegabytes 100.0)
        EngineOnline.New {Version = Version(3,2,1); DotNetSupport = Mono} "test" (FileSize.FromMegabytes 99.0)
    ] |> Set.ofSeq
    EngineInstalls =
        ActiveSet.createFrom [
            activeEngineInstall
            EngineInstall.New {Version = Version(11,88,0); DotNetSupport = DotNetCore} (DirectoryData.current()) godotFile
        ]
            activeEngineInstall
        |> unwrap
    Projects = [
        {
            Name = ProjectName "P1"
            Path = DirectoryData.current() |> ProjectDirectory
            File = godotFile |> ProjectFile
            AssociatedEngine =
                EngineInstall.New
                    {Version = Version(3,2,1); DotNetSupport = Mono}
                    (DirectoryData.current())
                    godotFile
                |> Some
        }
    ] |> Set.ofSeq
}

[<TearDown>]
let public Cleanup () =
    (DirectoryData.tryCreate AppDataPath |> unwrap).TryDelete() |> unwrap

[<Test>]
let public ``App State Is Loaded With The Same Data It Was Saved With`` () =
    let plugin = newPlugin ()
    plugin.Save(testAppState) |> unwrap
    let loaded = plugin.Load () |> unwrap
    Assert.That(loaded, Is.EqualTo(testAppState))

[<Test>]
let public ``Empty App State Can Be Saved And Loaded`` () =
    let empty = {
        EnginesOnline = Set.empty
        EngineInstalls = ActiveSet.createEmpty ()
        Projects = Set.empty
    }
    let plugin = newPlugin ()
    plugin.Save(empty) |> unwrap
    let loaded = plugin.Load () |> unwrap
    Assert.That(loaded, Is.EqualTo(empty))
