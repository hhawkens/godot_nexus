namespace App.Core.PluginDefinitions

open App.Core.Domain

// Godot-project related plugins.

type public UProjectsDirectoryGetter = unit -> ProjectsDirectory
type public UCreateNewProject = ProjectsDirectory -> ICreateNewProjectJob
type public UAddExistingProject = ProjectFile -> Result<Project, ErrorMessage>
type public URemoveProject = Project -> SimpleResult
type public UOpenProject = Project -> SimpleResult
