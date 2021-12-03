module public App.Shell.Plugins.DownloadEngine

open App.Core.Domain
open App.Core.PluginDefinitions
open FSharpPlus

let private downloadEngine cacheDirectory engine =
    DownloadEngineJob(cacheDirectory, engine, Web.downloadFileAsync):>IDownloadEngineJob

let public plugin: UDownloadEngine = downloadEngine
