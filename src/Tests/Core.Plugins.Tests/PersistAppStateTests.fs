module public App.Core.Plugins.Tests.PersistAppStateTests

open System
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Core.Plugins
open App.Utilities
open NUnit.Framework

let private newPlugin () =
    let plugin = PersistAppState():>UPersistAppState
    plugin.TryInitialize() |> unwrap
    plugin

let activeEngineInstall =
    EngineInstall.New {Version = Version(7,7,7); DotNetSupport = Mono} DirectoryData.Current

let private testAppState = {
    Engines = [
        Engine.New {Version = Version(1,2,3); DotNetSupport = NoSupport} "url" (FileSize.FromMegabytes 100.0)
        Engine.New {Version = Version(3,2,1); DotNetSupport = Mono} "test" (FileSize.FromMegabytes 99.0)
    ] |> Set.ofSeq
    EngineInstalls =
        ActiveSet.createFrom [
            activeEngineInstall
            EngineInstall.New {Version = Version(11,88,0); DotNetSupport = DotNetCore} DirectoryData.Current
        ]
            activeEngineInstall
        |> unwrap
    Projects = [
        {
            Name = ProjectName "P1"
            Path = DirectoryData.Current
            AssociatedEngine =
                EngineInstall.New
                    {Version = Version(3,2,1); DotNetSupport = Mono}
                    DirectoryData.Current
                |> Some
        }
    ] |> Set.ofSeq
}

[<TearDown>]
let public Cleanup () =
    (DirectoryData.TryCreate AppDataPath |> unwrap).TryDelete() |> unwrap

[<Test>]
let public ``App State Is Loaded With The Same Data It Was Saved With`` () =
    let plugin = newPlugin ()
    plugin.Save(testAppState) |> unwrap
    let loaded = plugin.Load () |> unwrap
    Assert.That(loaded, Is.EqualTo(testAppState))

[<Test>]
let public ``Empty App State Can Be Saved And Loaded`` () =
    let empty = {
        Engines = Set.empty
        EngineInstalls = ActiveSet.createEmpty ()
        Projects = Set.empty
    }
    let plugin = newPlugin ()
    plugin.Save(empty) |> unwrap
    let loaded = plugin.Load () |> unwrap
    Assert.That(loaded, Is.EqualTo(empty))
