module public App.Core.Plugins.Tests.PersistPreferencesTests

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Core.Plugins
open App.Utilities
open NUnit.Framework

[<Literal>]
let private prefsFile = "UserSettings.ini"

let private enginesPath = $"{AppDataPath}/engines" |> DirectoryData.tryCreate |> unwrap
let private projectsPath = $"{AppDataPath}/projects" |> DirectoryData.tryCreate |> unwrap

let private defaultPreferences = {
    General = {
        EnginesPath = {
            Description = "engines description"
            DefaultValue = DirectoryData.current()
            CurrentValue = DirectoryData.current()
        }
        ProjectsPath = {
            Description = "projects description"
            DefaultValue = DirectoryData.current()
            CurrentValue = DirectoryData.current()
        }
    }
    Ui = {
        Theme = {
            Description = "theme description"
            DefaultValue = Theme.System
            CurrentValue = Theme.System
        }
    }
}

let private testPreferences = {
    General = {
        EnginesPath = {
            Description = "alt"
            DefaultValue = enginesPath
            CurrentValue = enginesPath
        }
        ProjectsPath = {
            Description = "alt"
            DefaultValue = projectsPath
            CurrentValue = projectsPath
        }
    }
    Ui = {
        Theme = {
            Description = "alt"
            DefaultValue = Theme.Dark
            CurrentValue = Theme.Dark
        }
    }
}

let private newPlugin () =
    let plugin = PersistPreferences(defaultPreferences) :> UPersistPreferences
    plugin.TryInitialize() |> unwrap
    plugin

[<TearDown>]
let public Cleanup () =
    (DirectoryData.tryCreate AppDataPath |> unwrap).TryDelete() |> unwrap

[<Test>]
let public ``Valid Saved And Loaded Prefs Have Consistent Data`` () =
    let plugin = newPlugin()
    plugin.Save testPreferences |> unwrap
    match plugin.Load() with
    | Loaded prefs ->
        Assert.That(prefs.General.EnginesPath.Description, Is.EqualTo("engines description"))
        Assert.That(prefs.General.EnginesPath.DefaultValue, Is.EqualTo(DirectoryData.current()))
        Assert.That(prefs.General.EnginesPath.CurrentValue, Is.EqualTo(enginesPath))

        Assert.That(prefs.General.ProjectsPath.Description, Is.EqualTo("projects description"))
        Assert.That(prefs.General.ProjectsPath.DefaultValue, Is.EqualTo(DirectoryData.current()))
        Assert.That(prefs.General.ProjectsPath.CurrentValue, Is.EqualTo(projectsPath))

        Assert.That(prefs.Ui.Theme.Description, Is.EqualTo("theme description"))
        Assert.That(prefs.Ui.Theme.DefaultValue, Is.EqualTo(Theme.System))
        Assert.That(prefs.Ui.Theme.CurrentValue, Is.EqualTo(Theme.Dark))
    | _ ->
        Assert.Fail("Preferences were not loaded!")

[<Test>]
let public ``Loading Preferences Without Existing File Yields Defaults`` () =
    let plugin = newPlugin()
    match plugin.Load() with
    | LoadedDefaults prefs ->
        Assert.That(prefs, Is.EqualTo(defaultPreferences))
    | _ ->
        Assert.Fail("Default preferences not loaded!")

[<Test>]
let public ``Loading Faulty File Yields Error`` () =
    let fileData = FileData.tryCreate (Path.Combine(AppDataPath, prefsFile)) |> unwrap
    File.WriteAllText(fileData.FullPath, "[Bla]\nBla=Bla")
    let plugin = newPlugin()
    match plugin.Load() with
    | LoadFailed _ -> Assert.Pass()
    | _ -> Assert.Fail("Expected loading of faulty ini file to fail!")
