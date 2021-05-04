namespace App.Core.Domain

/// A Job signifies a long-running process, usually IO-bound.
type public IJob =
    abstract Id: Id
    abstract Name: string
    abstract Status: JobStatus
    abstract Updated: IJob IEvent
    abstract Abort: unit -> unit

/// A Job signifies a long-running process, usually IO-bound.
type public IJob<'input, 'output, 'error> =
    inherit IJob
    abstract Run: 'input -> unit Async
    abstract EndStatus: JobEndStatus<'output, 'error>
