namespace App.Shell.Plugins

open App.Core.Domain

type public SimpleCreateProjectJob (createProjectAction) =

    let id = generateJobId<SimpleCreateProjectJob> ()
    let updated = Event<IJob> ()
    let mutable status = Waiting
    let mutable endStatus = NotEnded

    interface ICreateNewProjectJob with

        member this.Id = id
        member this.Name = nameof DownloadEngineJob
        member this.Status = status
        member this.EndStatus = endStatus
        member this.Updated = updated.Publish

        member this.Abort () = () // Aborting not feasible at the moment

        member this.Run () = async {
            status <- Running {Action = "Creating new Project"; Progress = None}
            updated.Trigger this
            let newProjectResult = createProjectAction ()
            status <- Ended
            match newProjectResult with
            | Ok project -> endStatus <- Succeeded project
            | Error err -> endStatus <- Failed err
            updated.Trigger this
        }
