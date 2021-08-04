namespace App.Core.PluginDefinitions

open App.Core.Domain

/// Marks a system plugin. Plugins are extensions to the core,
/// called by the core itself.
/// Can be objects or simple functions.
/// The "U" prefix marks plugin definitions.
type public UPlugin =

    /// Called once before using the plugin. If an error occurs, the app cannot start / continue.
    abstract TryInitialize: unit -> SimpleResult
