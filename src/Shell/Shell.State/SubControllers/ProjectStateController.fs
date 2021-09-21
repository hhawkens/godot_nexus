namespace App.Shell.State

open App.Core.Domain
open App.Core.PluginDefinitions

/// Manages projects state manipulation.
type public IProjectStateController =
    abstract CreateNewProject: ProjectName -> unit
    abstract AddExistingProject: ProjectFile -> SimpleResult
    abstract RemoveProject: Project -> SimpleResult
    abstract OpenProject: Project -> SimpleResult


/// Manages projects state manipulation.
type public ProjectStateController
    (projectsDirectoryPlugin: UProjectsDirectoryGetter,
     createNewProjectPlugin: UCreateNewProject,
     addExistingProjectPlugin: UAddExistingProject,
     removeProjectPlugin: URemoveProject,
     openProjectPlugin: UOpenProject,
     jobsController: IJobsController,
     appStateInstance: AppStateInstance) =

    let projectsDir = projectsDirectoryPlugin()

    let state () = appStateInstance.State
    let setState appState = appStateInstance.SetState appState

    let addProject project =
        let newState = {state() with Projects = state().Projects.Add project}
        setState newState

    let removeProject project =
        let newState = {state() with Projects = state().Projects.Remove project}
        setState newState

    interface IProjectStateController with

        member this.CreateNewProject name =
            let job = createNewProjectPlugin projectsDir name
            jobsController.AddJob (CreateProject job)
            job.Run () |> Async.StartChild |> ignore

        member this.AddExistingProject file =
            match addExistingProjectPlugin file with
            | Ok project -> Ok (addProject project)
            | Error err -> Error err

        member this.RemoveProject project =
            match removeProjectPlugin project with
            | SuccessfulRemoval -> removeProject project |> Ok
            | NotFound ->
                removeProject project
                Error $"Could not remove project {project}, folder not found!"
            | RemovalFailed err -> Error err

        member this.OpenProject project =
            match project.AssociatedEngine, appStateInstance.State.EngineInstalls.Active with
            | Some engine, _ | _, Some engine -> openProjectPlugin engine project
            | None, None -> Error $"Cannot open {project} because no engine was found!"
