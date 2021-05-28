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

let shouldExist (layer: GivenTypesConjunctionWithDescription) =
    let layerObjects = architecture |> layer.GetObjects |> toList
    Assert.That(layerObjects.Length, Is.GreaterThan(0))

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
    let frontendLayer = Types().That().ResideInAssembly(@"App.Presentation.Frontend").As("Frontend Layer")
    let guiLayer = Types().That().ResideInAssembly(@"App.Presentation.Gui").As("GUI Layer")
    let mainLayer = Types().That().ResideInAssembly(@"App.Main*", true).As("Main Layer")

    utilsLayer |> shouldExist
    coreLayer |> shouldExist
    frontendLayer |> shouldExist
    guiLayer |> shouldExist
    mainLayer |> shouldExist

    utilsLayer |> shouldNotDependOn coreLayer
    utilsLayer |> shouldNotDependOn frontendLayer
    utilsLayer |> shouldNotDependOn guiLayer
    utilsLayer |> shouldNotDependOn mainLayer

    coreLayer |> shouldNotDependOn frontendLayer
    coreLayer |> shouldNotDependOn guiLayer
    coreLayer |> shouldNotDependOn mainLayer

    frontendLayer |> shouldNotDependOn mainLayer
    frontendLayer |> shouldNotDependOn guiLayer

    guiLayer |> shouldNotDependOn mainLayer
    guiLayer |> shouldNotDependOn coreLayer // This is because we are using closed layers
