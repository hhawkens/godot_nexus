module public App.SystemTests.ArchitectureTests

open ArchUnitNET.Fluent;
open ArchUnitNET.Fluent.Syntax.Elements.Types
open ArchUnitNET.Loader;
open ArchUnitNET.NUnit;
open NUnit.Framework
open FSharpPlus
open App.Core.State
open App.TestHelpers
open type ArchRuleDefinition

type public SystemAssembly = System.Reflection.Assembly

// These bindings only exist to actually use assemblies, so they are properly referenced
let _ = App.Main.ProgramEntry.entry
let _ = {PropertyName = ""; PropertyType = typedefof<int>}

let mutable architecture = ArchLoader().Build()

let shouldNotDependOn (b: GivenTypesConjunctionWithDescription) (a: GivenTypesConjunctionWithDescription) =
    let forbiddenLayerAccess = a.Should().NotDependOnAny(b)
    forbiddenLayerAccess.Check architecture

[<OneTimeSetUp>]
let public OneTimeSetUp () =
    let allProjectAssemblies =
        SystemAssembly.GetExecutingAssembly() |> getReferencedAssemblies |> toArray |> sortBy (fun x -> x.FullName)
    architecture <- ArchLoader().LoadAssemblies(allProjectAssemblies).Build()

[<Test>]
let public ``All Layers Only Reference Lower Level Layers`` () =

    let utilsLayer = Types().That().ResideInAssembly(@"App.Utilities*", true).As("Utils Layer")
    let coreLayer = Types().That().ResideInAssembly(@"App.Core*", true).As("Core Layer")
    let presentationLayer = Types().That().ResideInAssembly(@"App.Presentation*", true).As("Presentation Layer")
    let mainLayer = Types().That().ResideInAssembly(@"App.Main*", true).As("Main Layer")

    utilsLayer |> shouldNotDependOn coreLayer
    utilsLayer |> shouldNotDependOn presentationLayer
    utilsLayer |> shouldNotDependOn mainLayer

    coreLayer |> shouldNotDependOn presentationLayer
    coreLayer |> shouldNotDependOn mainLayer

    presentationLayer |> shouldNotDependOn mainLayer
