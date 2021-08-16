namespace App.Shell.State

open App.Core.Domain
open App.Core.PluginDefinitions

type public ProjectStateController
    (projectsDirectoryPlugin: UProjectsDirectoryGetter,
     createNewProjectPlugin: UCreateNewProject,
     addExistingProjectPlugin: UAddExistingProject,
     removeProjectPlugin: URemoveProject,
     openProjectPlugin: UOpenProject,
     jobsController: JobsController,
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

    member public this.CreateNewProject name =
        let job = createNewProjectPlugin projectsDir name
        jobsController.AddJob (CreateProject job)
        job.Run () |> Async.StartChild |> ignore

    member public this.AddExistingProject file =
        match addExistingProjectPlugin file with
        | Ok project -> Ok (addProject project)
        | Error err -> Error err

    member public this.RemoveProject project =
        match removeProjectPlugin project with
        | SuccessfulRemoval -> removeProject project |> Ok
        | NotFound ->
            removeProject project
            Error $"Could not remove project {project}, folder not found!"
        | RemovalFailed err -> Error err

    member public this.OpenProject project =
        openProjectPlugin project
