namespace App.Shell.State

open System

type public ChangedProperty = {
    PropertyName: string
    PropertyType: Type
}

type public AppStateChangedArgs = {
    ChangedProperties: ChangedProperty array
}
