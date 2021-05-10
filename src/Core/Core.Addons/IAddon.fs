namespace App.Core.Addons

open System
open App.Core.State

/// Defines all functionality an addon must provide.
type public IAddon =
    inherit IComparable

    /// Name or specifier for addon. Must be unique per process.
    abstract Id: string

    /// Called before app is initialized.
    abstract BeforeInitializeTask: unit AddonTask option

    /// Called after app is initialized.
    abstract AfterInitializeTask: IAppStateController AddonTask option

    /// Called repeatedly in certain intervals.
    abstract TickTask: TickTask option
