namespace App.Shell.State

open App.Core.Domain
open FSharpPlus

/// Manages all jobs the application is running.
type public IJobsController =
    abstract JobStarted: IEvent<JobDef>
    abstract AddJob: JobDef -> unit
    abstract AbortJob: Id -> SimpleResult


/// Manages all jobs the application is running.
type internal JobsController () =

    let jobStarted = Event<JobDef>()

    let mutable jobs = Map<Id, JobDef>([])

    let threadSafe = threadSafeFactory ()

    let removeJobIfEnded (job: IJob) = threadSafe (fun () ->
        match job.Status with
        | Ended -> jobs <- jobs.Remove job.Id
        | _ -> ()
    )

    let addJob (jobDef: JobDef) =
        jobs <- jobs.Add (jobDef.Job.Id, jobDef)
        jobDef.Job.Updated.Add removeJobIfEnded
        jobStarted.Trigger jobDef

    interface IJobsController with

        member this.JobStarted = jobStarted.Publish

        member this.AddJob jobDef =
            threadSafe (fun () -> addJob jobDef)

        member this.AbortJob id =
            match jobs |> Map.tryFind id with
            | Some jobDef -> jobDef.Job.Abort() |> Ok
            | None -> Error $"Cannot abort job {id}, not found!"
