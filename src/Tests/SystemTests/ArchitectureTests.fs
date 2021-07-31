module public App.SystemTests.ArchitectureTests

open ArchUnitNET.Fluent;
open ArchUnitNET.Fluent.Syntax.Elements.Types
open ArchUnitNET.Loader;
open ArchUnitNET.NUnit;
open NUnit.Framework
open FSharpPlus
open App.Shell.State
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
    let shellLayer = Types().That().ResideInAssembly(@"App.Shell*", true).As("Shell Layer")
    let frontendLayer = Types().That().ResideInAssembly(@"App.Presentation.Frontend").As("Frontend Layer")
    let guiLayer = Types().That().ResideInAssembly(@"App.Presentation.Gui").As("GUI Layer")
    let mainLayer = Types().That().ResideInAssembly(@"App.Main*", true).As("Main Layer")

    utilsLayer |> shouldExist
    coreLayer |> shouldExist
    shellLayer |> shouldExist
    frontendLayer |> shouldExist
    guiLayer |> shouldExist
    mainLayer |> shouldExist

    utilsLayer |> shouldNotDependOn coreLayer
    utilsLayer |> shouldNotDependOn shellLayer
    utilsLayer |> shouldNotDependOn frontendLayer
    utilsLayer |> shouldNotDependOn guiLayer
    utilsLayer |> shouldNotDependOn mainLayer

    coreLayer |> shouldNotDependOn shellLayer
    coreLayer |> shouldNotDependOn frontendLayer
    coreLayer |> shouldNotDependOn guiLayer
    coreLayer |> shouldNotDependOn mainLayer

    shellLayer |> shouldNotDependOn frontendLayer
    shellLayer |> shouldNotDependOn guiLayer
    shellLayer |> shouldNotDependOn mainLayer

    frontendLayer |> shouldNotDependOn mainLayer
    frontendLayer |> shouldNotDependOn guiLayer

    guiLayer |> shouldNotDependOn mainLayer
    // The following are necessary because we are using closed layers
    guiLayer |> shouldNotDependOn coreLayer
    guiLayer |> shouldNotDependOn shellLayer
