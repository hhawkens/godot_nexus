namespace App.Shell.State

open FSharpPlus
open App.Core.Domain
open App.Utilities

/// Encapsulates the functionality to mutate the app state.
type public AppStateInstance (state: AppState) =

    let mutable state = state

    let stateChanged = Event<AppStateChangedArgs>()

    let threadSafe = threadSafeFactory ()

    let setState = function
        | newState when newState <> state ->
            let diffs = (Compare.allPropertyDiffs state newState)
            state <- newState
            let changed = diffs |> map (fun diff -> {PropertyName = diff.Name; PropertyType = diff.DeclaringType})
            stateChanged.Trigger {ChangedProperties = changed}
        | _ -> ()

    member public this.StateChanged = stateChanged.Publish

    member public this.State = state

    member public this.SetState appState =
        threadSafe (fun () -> setState appState)
