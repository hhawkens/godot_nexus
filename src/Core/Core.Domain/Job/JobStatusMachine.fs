namespace App.Core.Domain

/// Describes the possible transition between job statuses.
type public JobStatusMachine<'success, 'error> = private {
    status: JobStatus
    endStatus: JobEndStatus<'success, 'error>
} with

    member public this.Status = this.status
    member public this.EndStatus = this.endStatus;

    static member public New (): JobStatusMachine<'a, 'b> =
        {status = Waiting; endStatus = NotEnded}

    static member public Transition nextStatus statusMachine =
        match statusMachine.status, nextStatus with
        | Ended, _ | _, Ended -> None
        | _, next -> Some {statusMachine with status = next}

    static member public Conclude endStatus statusMachine =
        match statusMachine.status, endStatus with
        | Ended, _ | _, NotEnded -> None
        | _ -> Some {status = Ended; endStatus = endStatus}
