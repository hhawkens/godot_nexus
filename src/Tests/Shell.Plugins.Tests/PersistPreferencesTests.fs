module public App.Shell.Plugins.Tests.PersistPreferencesTests

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Shell.Plugins
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
    UI = {
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
    UI = {
        Theme = {
            Description = "alt"
            DefaultValue = Theme.Dark
            CurrentValue = Theme.Dark
        }
    }
}

let private newSut () =
    let plugin = PersistPreferences(defaultPreferences) :> UPersistPreferences
    plugin.TryInitialize() |> unwrap
    plugin

[<TearDown>]
let public Cleanup () =
    (DirectoryData.tryCreate AppDataPath |> unwrap).TryDelete() |> unwrap

[<Test>]
let public ``Valid Saved And Loaded Prefs Have Consistent Data`` () =
    let sut = newSut()
    sut.Save testPreferences |> unwrap
    match sut.Load() with
    | Loaded prefs ->
        Assert.That(prefs.General.EnginesPath.Description, Is.EqualTo("engines description"))
        Assert.That(prefs.General.EnginesPath.DefaultValue, Is.EqualTo(DirectoryData.current()))
        Assert.That(prefs.General.EnginesPath.CurrentValue, Is.EqualTo(enginesPath))

        Assert.That(prefs.General.ProjectsPath.Description, Is.EqualTo("projects description"))
        Assert.That(prefs.General.ProjectsPath.DefaultValue, Is.EqualTo(DirectoryData.current()))
        Assert.That(prefs.General.ProjectsPath.CurrentValue, Is.EqualTo(projectsPath))

        Assert.That(prefs.UI.Theme.Description, Is.EqualTo("theme description"))
        Assert.That(prefs.UI.Theme.DefaultValue, Is.EqualTo(Theme.System))
        Assert.That(prefs.UI.Theme.CurrentValue, Is.EqualTo(Theme.Dark))
    | _ ->
        Assert.Fail("Preferences were not loaded!")

[<Test>]
let public ``Loading Preferences Without Existing File Yields Defaults`` () =
    let sut = newSut()
    match sut.Load() with
    | LoadedDefaults prefs ->
        Assert.That(prefs, Is.EqualTo(defaultPreferences))
    | _ ->
        Assert.Fail("Default preferences not loaded!")

[<Test>]
let public ``Loading Faulty File Yields Error`` () =
    let fileData = FileData.tryCreate (Path.Combine(AppDataPath, prefsFile)) |> unwrap
    File.WriteAllText(fileData.FullPath, "[Bla]\nBla=Bla")
    let sut = newSut()
    match sut.Load() with
    | LoadFailed _ -> Assert.Pass()
    | _ -> Assert.Fail("Expected loading of faulty ini file to fail!")
