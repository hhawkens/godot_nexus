namespace App.Utilities

open System.Collections.Generic

type public PropertyName = string

/// Describes a type the emits an event whenever one of its properties changes.
type public IPropertyChanged =
    abstract PropertyChanged: PropertyName IReadOnlyList IEvent
