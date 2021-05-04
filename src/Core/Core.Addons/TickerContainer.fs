namespace App.Core.Addons

open App.Utilities

type internal TickerContainer (tickTask) =

    let mutable ticksSinceExecution = tickTask.ExecuteEvery.Count
    let mutable isExecuting = false

    let threadSafe = threadSafeFactory ()

    let setTicks newVal = ticksSinceExecution <- newVal
    let alterTicks fn = setTicks (fn ticksSinceExecution)
    let resetTicks () = setTicks 1u

    let callAddonTask tick appStateManager =
        match tickTask.Task with
            | AddonSync task ->
                task (tick, appStateManager)
                resetTicks ()
            | AddonAsync task ->
                isExecuting <- true
                Async.Start (async {
                    let! _ = task (tick, appStateManager)
                    isExecuting <- false
                    threadSafe resetTicks
                })

    let update tick appState =
        if isExecuting then
            ()
        else if ticksSinceExecution >= tickTask.ExecuteEvery.Count then do
            callAddonTask tick appState
        else do
            alterTicks ((+)1u)

    member public this.Update tick appStateManager =
        threadSafe (fun _ -> update tick appStateManager)
