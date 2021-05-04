namespace App.Core.PluginDefinitions

open App.Core.Domain

/// Plugin used to save and load app state on disk.
type public UPersistAppState =
    inherit UPlugin
    abstract Save: IAppState -> SimpleResult
    abstract Load: unit -> Result<AppState, ErrorMessage>
