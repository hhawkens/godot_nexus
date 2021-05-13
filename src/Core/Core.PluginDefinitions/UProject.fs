namespace App.Core.PluginDefinitions

open App.Core.Domain

type public UProjectsDirectoryGetter = unit -> ProjectsDirectory
type public UCreateNewProject = ProjectsDirectory -> ICreateNewProjectJob
type public UAddExistingProject = ProjectFile -> Result<Project, ErrorMessage>
type public URemoveProject = Project -> SimpleResult
type public UOpenProject = Project -> SimpleResult
