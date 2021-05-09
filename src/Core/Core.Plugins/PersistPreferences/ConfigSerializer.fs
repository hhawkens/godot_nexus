module internal App.Core.Plugins.ConfigSerializer

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities
open SharpConfig

[<Literal>]
let GeneralSection = "General"
[<Literal>]
let UiSection = "UI"

[<Literal>]
let EnginesPathSetting = "EnginesPath"
[<Literal>]
let ProjectsPathSetting = "ProjectsPath"
[<Literal>]
let ThemeSetting = "Theme"

let internal Save (file: FileData) prefs =
    let sharpConfig = Configuration()
    sharpConfig.[GeneralSection].[EnginesPathSetting].StringValue <-
        prefs.General.EnginesPath.CurrentValue.FullPath
    sharpConfig.[GeneralSection].[ProjectsPathSetting].StringValue <-
        prefs.General.ProjectsPath.CurrentValue.FullPath
    sharpConfig.[UiSection].[ThemeSetting].StringValue <-
        prefs.Ui.Theme.CurrentValue.ToString()

    try Ok (sharpConfig.SaveToFile file.FullPath)
    with | ex -> Error ex.Message

let private UnsafeLoadFrom defaultPrefs (file: FileData) =
    let sharpConfig = Configuration.LoadFromFile file.FullPath
    let enginesPath = sharpConfig.[GeneralSection].[EnginesPathSetting].StringValue
    let projectsPath = sharpConfig.[GeneralSection].[ProjectsPathSetting].StringValue
    let theme = sharpConfig.[UiSection].[ThemeSetting].StringValue
    {
        General = {
            EnginesPath = {
                Description = defaultPrefs.General.EnginesPath.Description
                DefaultValue = defaultPrefs.General.EnginesPath.DefaultValue
                CurrentValue = enginesPath |> DirectoryData.TryCreate |> unwrap
            }
            ProjectsPath = {
                Description = defaultPrefs.General.ProjectsPath.Description
                DefaultValue = defaultPrefs.General.ProjectsPath.DefaultValue
                CurrentValue = projectsPath |> DirectoryData.TryCreate |> unwrap
            }
        }
        Ui = {
            Theme = {
                Description = defaultPrefs.Ui.Theme.Description
                DefaultValue = defaultPrefs.Ui.Theme.DefaultValue
                CurrentValue = theme |> Enums.tryParse<Theme> |> unwrap
            }
        }
    }

let internal LoadFrom fallBack (file: FileData) =
    if File.Exists file.FullPath |> not || FileInfo(file.FullPath).Length = 0L then
        LoadedDefaults fallBack
    else
        match exnToResult (fun () -> UnsafeLoadFrom fallBack file) with
        | Ok prefs -> Loaded prefs
        | Error err -> LoadFailed err
