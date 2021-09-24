namespace App.Utilities

open System

/// Standard delegate for destructible events without any specific data.
type public DestructibleEventHandler = delegate of obj * EventArgs -> unit

/// Disposable with an event.
type public IDestructible =
    inherit IDisposable

    [<CLIEvent>]
    abstract Disposed: IEvent<DestructibleEventHandler, EventArgs>
