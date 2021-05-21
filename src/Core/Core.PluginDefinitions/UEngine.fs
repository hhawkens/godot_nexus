namespace App.Core.PluginDefinitions

open App.Core.Domain

// Engine related plugins.

type public UEnginesDirectoryGetter = unit -> EnginesDirectory
type public UDownloadEngine = CacheDirectory -> EngineOnline -> IDownloadEngineJob
type public UInstallEngine = EngineZipFile -> EnginesDirectory -> EngineOnline -> IInstallEngineJob
type public URemoveEngine = EngineInstall -> SimpleResult
type public URunEngine = EngineInstall -> SimpleResult
