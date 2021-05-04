namespace App.Core.PluginDefinitions

open App.Core.Domain

/// Plugin used to cache temporary files.
type public UCaching =
    inherit UPlugin
    abstract CacheDirectory: CacheDirectory
    abstract Clean: unit -> SimpleResult
