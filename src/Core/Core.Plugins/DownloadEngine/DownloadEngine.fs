module public App.Core.Plugins.DownloadEngine

open App.Core.Domain
open App.Core.PluginDefinitions
open App.Utilities

let private downloadEngine cacheDirectory =
    DownloadEngineJob(cacheDirectory, Web.downloadFileAsync):>IDownloadEngineJob

let public plugin: UDownloadEngine = downloadEngine
