module public App.Shell.Plugins.CreateNewProject_v3

open System.IO
open App.Core.Domain
open App.Core.PluginDefinitions

[<Literal>]
let private GodotProjectFile = "project.godot"
[<Literal>]
let private DefaultEnvFile = "default_env.tres"
[<Literal>]
let private IconFile = "icon.png"

/// Can throw exceptions, hence "Unsafe"
let private createUnsafe (directory: ProjectsDirectory) (name: ProjectName) =
    use godotProjectFileStream = File.CreateText (Path.Combine(directory.Val.FullPath, GodotProjectFile))
    let godotProjectText = ProjectFilesText_v3.GodotProjectText.Replace("%NAME%", name.Val)
    godotProjectFileStream.Write godotProjectText

    use defaultEnvFileStream = File.CreateText (Path.Combine(directory.Val.FullPath, DefaultEnvFile))
    defaultEnvFileStream.Write ProjectFilesText_v3.DefaultEnvText

    File.Copy (Path.Combine("PluginResources", IconFile), Path.Combine(directory.Val.FullPath, IconFile))
    {Name = name; Path = directory.Val; AssociatedEngine = None}

let private create directory name () =
    try
        let newProject = createUnsafe directory name
        Ok newProject
    with | ex ->
        Error ex.Message

let private createNewProjectWithJob directory name =
    SimpleCreateProjectJob (create directory name):>ICreateNewProjectJob

let public plugin: UCreateNewProject = createNewProjectWithJob
