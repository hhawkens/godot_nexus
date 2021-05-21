module public App.SystemTests.ExplicitVisibilityTests

open System
open System.IO
open System.Text.RegularExpressions
open App.Utilities
open NUnit.Framework
open FSharpPlus
open App.TestHelpers

let private __ = Path.DirectorySeparatorChar

let private findErrorsInFile (file: FileData) =
    let moduleRegex = @"(?<!\/\/.*)\bmodule\s+(?!private|public|internal)" |> Regex
    let typeRegex = @"(?<!(?:\/\/|open).*)\btype\s+(?!private|public|internal)" |> Regex
    let andRegex = @"(?<!\/\/.*)\band\s+(?!private|public|internal)" |> Regex
    let fileLines = File.ReadAllText file.FullPath |> String.split [Environment.NewLine]

    fileLines
    |> Seq.indexed
    |> choose (fun (idx, line) ->
        if moduleRegex.IsMatch(line) || typeRegex.IsMatch(line) || andRegex.IsMatch(line) then
            Some $"{file.FullPath}:line {idx + 1}"
        else
            None
        )

[<Test>]
let public ``Check If All F# Modules And Types Have Explicit Visibility Specified`` () =
    let allErrors =
        msBuildSolutionFolder.FindFilesRecWhere (fun f -> f.Extension = ".fs")
        |> filter (fun f -> not <| f.FullPath.Contains($"{__}obj{__}"))
        |> foldMap findErrorsInFile
        |> toList
    if allErrors.Length = 0 then
        Assert.Pass()
    else
        Assert.Fail($"F# files with missing Type / Module visibility specifier:\n{formatSeq allErrors}\n\n")
