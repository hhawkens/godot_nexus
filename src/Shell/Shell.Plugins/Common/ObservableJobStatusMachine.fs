namespace App.Shell.Plugins

open App.Core.Domain
open App.Utilities

type internal ObservableJobStatusMachine<'success, 'error> (threadSafety, job) =

    let mutable statusMachine = JobStatusMachine.New<'success, 'error> ()

    let updated = Event<IJob> ()

    let threadSafetyOption =
        match threadSafety with
        | ThreadSafe -> threadSafeFactory ()
        | ThreadUnsafe -> (fun f -> f())

    let setStatusGeneric setter newStatus =
        threadSafetyOption (fun () ->
            match (setter newStatus statusMachine) with
            | Some sm ->
                statusMachine <- sm
                updated.Trigger job
            | _ -> ())

    member public this.Updated = updated.Publish
    member public this.Status = statusMachine.Status
    member public this.EndStatus = statusMachine.EndStatus

    member public this.SetStatus newStatus =
        setStatusGeneric JobStatusMachine<_,_>.Transition newStatus

    member public this.SetEndStatus endStatus =
        setStatusGeneric JobStatusMachine<_,_>.Conclude endStatus
