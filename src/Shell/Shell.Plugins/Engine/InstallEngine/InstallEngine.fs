module public App.Shell.Plugins.InstallEngine

open System.Text.RegularExpressions
open App.Core.Domain
open App.Core.PluginDefinitions

let private installEngine regex zipFile dir engineData =
    InstallEngineJob(zipFile, dir, engineData, regex):>IInstallEngineJob

let public plugin: UInstallEngine = installEngine (Regex "Godot*")
