module public App.Utilities.Tests.SystemsTests

open System
open System.IO
open System.Text.RegularExpressions
open FSharpPlus
open NUnit.Framework
open App.Utilities
open App.TestHelpers

[<Literal>]
let private RootNameSpace = "App"

let private __ = Path.DirectorySeparatorChar

[<Test>]
let public ``Check If All F# Modules And Types Have Explicit Visibility Specified`` () =
    let findErrorsInFile (file: FileData) =
        let moduleRegex = @"(?<!\/\/.*)\bmodule\s+(?!private|public|internal)" |> Regex
        let typeRegex = @"(?<!\/\/.*)\btype\s+(?!private|public|internal)" |> Regex
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

    let allErrors =
        msBuildSolutionFolder.FindFilesRecWhere (fun f -> f.Extension = ".fs")
        |> filter (fun f -> not <| f.FullPath.Contains($"{__}obj{__}"))
        |> foldMap findErrorsInFile
        |> toList
    if allErrors.Length = 0 then
        Assert.Pass()
    else
        Assert.Fail($"F# files with missing Type / Module visibility specifier:\n{formatSeq allErrors}\n\n")

[<Test>]
let public ``Check If All Code Files Use The Correct Name Space`` () =
    let namespaceRegex = $@"\bnamespace\s+([\w\d.]+?)\s" |> Regex
    let moduleRegex = $@"\bmodule\s+(?:\w+\s)*((?:[\w]+\.)+)" |> Regex

    let findError (projectName: string) (sourceFile: FileData) (regex: Regex) =
        let content = File.ReadAllText sourceFile.FullPath
        let match' = regex.Match(content)
        if match'.Success then
            let namespaceText = match'.Groups.[1].Value
            let namespaceText =
                if namespaceText.EndsWith "." then namespaceText.Substring(0, namespaceText.Length - 1)
                else namespaceText
            let expected = $"{RootNameSpace}.{projectName}"
            if expected <> namespaceText then
                Some $"Expected: {expected} - Actual: {namespaceText} - File: {sourceFile.FullPath}"
            else None
        else
            None

    let findAllNamespaceErrors (projectFile: FileData) (sourceFiles: FileData list) =
        sourceFiles
        |> choose (fun source ->
            let projectName = projectFile.Name.Replace(projectFile.Extension, "")
            maybe {
                let findErrorMatch = findError projectName source
                return! findErrorMatch namespaceRegex
                return! findErrorMatch moduleRegex
            })

    let projectFiles =
        msBuildSolutionFolder.FindFilesRecWhere (fun f -> f.Extension = ".csproj" || f.Extension = ".fsproj")

    Assert.That(projectFiles.Length, Is.GreaterThan(5)) // Arbitrary number, should be low enough to pass if correct

    let errors =
        projectFiles
        |> map (fun projectFile ->
            let sourceFiles =
                (DirectoryData.Of projectFile).FindFilesRecWhere (fun f ->
                    let isSourceFile = f.Extension = ".cs" || f.Extension = ".fs"
                    let isNotGenerated = not <| f.FullPath.Contains $"{__}obj{__}"
                    let isNoException = not <| f.Name.ToLower().Contains "fsharpplus"
                    isSourceFile && isNotGenerated && isNoException)
            findAllNamespaceErrors projectFile sourceFiles)
        |> flatten
        |> toList
    if errors.Length > 0 then do
        Assert.Fail(formatSeq errors)
    else
        Assert.Pass()
