namespace App.Utilities

type public PropertyName = string

/// Describes a type the emits an event whenever one of its properties changes.
type public IPropertyChanged =
    abstract PropertyChanged: PropertyName list IEvent
