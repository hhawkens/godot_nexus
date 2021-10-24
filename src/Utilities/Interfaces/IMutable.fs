namespace App.Utilities

type public PropertyName = string

/// Describes an object whose state is mutable and
/// which emits an event whenever its state changes.
type public IMutable =

    /// Called when the state of this object has been altered.
    abstract StateChanged: unit IEvent
