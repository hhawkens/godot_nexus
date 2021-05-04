namespace App.TestHelpers

open FSharpPlus
open App.Utilities

[<AutoOpen>]
module public Library =

    /// Finds the folder containing the .sln file of this project. Does not work in builds, only dev mode.
    let public msBuildSolutionFolder =
        let rec findSolutionFromDir (dir: DirectoryData) =
            if dir.Files |> exists (fun f -> f.Extension = ".sln") then
                dir
            else
                findSolutionFromDir (dir.Parent |> unwrap)
        findSolutionFromDir (System.Environment.CurrentDirectory |> DirectoryData.TryFind |> unwrap)
