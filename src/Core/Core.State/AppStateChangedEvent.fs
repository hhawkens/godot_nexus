namespace App.Core.State

open System

type public AppStateChangedArgs = {
    PropertyName: string
    PropertyType: Type
}
