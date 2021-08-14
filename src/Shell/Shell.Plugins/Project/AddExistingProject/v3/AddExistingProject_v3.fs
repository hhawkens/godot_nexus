module public App.Shell.Plugins.AddExistingProject_v3

open System.IO
open System.Text.RegularExpressions
open App.Core.PluginDefinitions
open App.Utilities
open FSharpPlus
open App.Core.Domain

let private findProjectName projectFileText =
    let nameMatch = Regex.Match(projectFileText, @"config\s*[\/\\]\s*name\s*=\s*""(.+?)""[\s\S]")
    match nameMatch.Groups.Count with
    | c when c >= 2 -> nameMatch.Groups.[1].Value |> Ok
    | _ -> Error "Project file does not contain project name!"

let private addProject (ProjectFile projectFile) =
    match projectFile.StillExists with
    | false -> Error "Could not add existing project, file does not exist!"
    | true ->
        let projectFileText = File.ReadAllText projectFile.FullPath
        monad.plus' {
            let! projectName = findProjectName projectFileText
            let projectPath = DirectoryData.from projectFile
            return {
                Name = projectName |> ProjectName
                Path = projectPath |> ProjectDirectory
                File = projectFile |> ProjectFile
                AssociatedEngine = None
            }
        }

let public plugin: UAddExistingProject = addProject
