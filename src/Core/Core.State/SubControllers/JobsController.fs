namespace App.Core.State

open App.Core.Domain
open App.Utilities

/// Manages all jobs the application is running.
type public JobsController () =

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

    member public this.JobStarted = jobStarted.Publish

    member public this.AddJob jobDef =
        threadSafe (fun () -> addJob jobDef)

    member public this.AbortJob id =
        match jobs |> Map.tryFind id with
        | Some jobDef -> jobDef.Job.Abort() |> Ok
        | None -> Error $"Cannot abort job {id}, not found!"
