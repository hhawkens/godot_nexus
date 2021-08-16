namespace App.Core.PluginDefinitions

open App.Core.Domain

type public UProjectsDirectoryGetter = unit -> ProjectDirectory
type public UCreateNewProject = ProjectDirectory -> ProjectName -> ICreateNewProjectJob
type public UAddExistingProject = ProjectFile -> Result<Project, ErrorMessage>
type public URemoveProject = Project -> RemovalResult
type public UOpenProject = Project -> SimpleResult
