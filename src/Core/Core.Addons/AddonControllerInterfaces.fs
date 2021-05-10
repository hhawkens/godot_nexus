namespace App.Core.Addons

open App.Core.Domain
open App.Core.State

/// Addons are extensions to the core that hook themselves into the application,
/// are called once or regularly.
type public IAddonHook =

    /// Registers an addon with the addon-event-system
    abstract RegisterAddon: IAddon -> unit


/// Manages + updates addon hooks.
type public IAddonController =

    /// Needs to be called by the system before it is initialized.
    abstract CallBeforeInitialize: unit -> unit

    /// Needs to be called by the system right after it is initialized.
    abstract CallAfterInitialize: IAppStateController -> unit

    /// Updates time based hooks.
    abstract Update: Tick -> IAppStateController -> unit

    /// Closes all running addons
    abstract ShutDown: unit -> SimpleResult
