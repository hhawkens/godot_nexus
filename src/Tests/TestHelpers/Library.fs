[<AutoOpen>]
module public App.TestHelpers.Library

open System.Reflection
open FSharpPlus
open App.Utilities

/// Finds the folder containing the .sln file of this project (DOES NOT WORK WITH BUILDS, ONLY DEV MODE).
let public msBuildSolutionFolder =
    let rec findSolutionFromDir (dir: DirectoryData) =
        if dir.Files |> exists (fun f -> f.Extension = ".sln") then
            dir
        else
            findSolutionFromDir (dir.Parent |> unwrap)
    findSolutionFromDir (System.Environment.CurrentDirectory |> DirectoryData.tryFind |> unwrap)

/// Get all project assemblies (excluding tests) referenced by given assembly, recursively.
let public getReferencedAssemblies assembly =
    let rec getAssembliesRec (asm: Assembly) =
        let subs =
            asm.GetReferencedAssemblies()
            |> filter (fun x -> x.FullName.StartsWith "App." && not <| x.FullName.Contains "Test")
            |>> Assembly.Load
            |> toList
        match subs with
        | [] -> []
        | subs -> subs @ (subs |>> getAssembliesRec |> flatten |> toList)
    getAssembliesRec assembly |> distinct
