namespace App.Core.State

open App.Core.Domain
open App.Utilities

/// Encapsulates the functionality to mutate the app state.
type public AppStateInstance (state: AppState) =

    let mutable state = state

    let stateChanged = Event<AppStateChangedArgs>()

    let threadSafe = threadSafeFactory ()

    let setState = function
        | newState when newState <> state ->
            let diff = (Compare.allPropertyDiffs state newState).[0] // TODO check for more than one change
            state <- newState
            stateChanged.Trigger {PropertyName = diff.Name; PropertyType = diff.DeclaringType}
        | _ -> ()

    member public this.StateChanged = stateChanged.Publish

    member public this.State = state

    member public this.SetState appState =
        threadSafe (fun () -> setState appState)
