namespace App.Core.Domain

open FSharpPlus

/// Describes what a running job is doing.
type public JobRunning = {
    Action: string
    Progress: Percent option
}

/// Describes the current state of a job.
type public JobStatus =
    | Waiting
    | Running of JobRunning
    | Aborting
    | Ended

/// Describes the state of a job after it has finished (one way or another).
type public JobEndStatus<'success, 'error> =
    | NotEnded
    | Aborted
    | Succeeded of 'success
    | Failed of 'error
