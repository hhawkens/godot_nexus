namespace App.Shell.Addons

open FSharpPlus

type internal TickerContainer (tickTask) =

    let mutable ticksSinceExecution = tickTask.ExecuteEvery.Count
    let mutable isExecuting = false

    let threadSafe = threadSafeFactory ()

    let setTicks newVal = ticksSinceExecution <- newVal
    let alterTicks fn = setTicks (fn ticksSinceExecution)
    let resetTicks () = setTicks 1u

    let callAddonTask tick appStateController =
        match tickTask.Task with
            | AddonSync task ->
                task (tick, appStateController)
                resetTicks ()
            | AddonAsync task ->
                isExecuting <- true
                Async.Start (async {
                    let! _ = task (tick, appStateController)
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

    member public this.Update tick appStateController =
        threadSafe (fun _ -> update tick appStateController)
