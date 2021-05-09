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

let private testAppState = {
    Engines = [
        Engine.New {Version = Version(1,2,3); DotNetSupport = NoSupport} "url" (FileSize.FromMegabytes 100.0)
        Engine.New {Version = Version(3,2,1); DotNetSupport = Mono} "test" (FileSize.FromMegabytes 99.0)
    ] |> Set.ofSeq
    EngineInstalls = ActiveSet.createEmpty ()
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
    match plugin.Save(testAppState) with
    | Ok _ -> ()
    | Error err -> Assert.Fail($"Saving failed with:\n{err}\n")
    let loaded = plugin.Load () |> unwrap

    Assert.That(loaded, Is.EqualTo(testAppState))
