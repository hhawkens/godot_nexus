namespace App.Core.Domain

open App.Core.Domain
open App.Utilities

type public IAppState =
    abstract Engines: Engines
    abstract EngineInstalls: EngineInstalls
    abstract Projects: Projects

and public AppState =
    {
        Engines: Engines
        EngineInstalls: EngineInstalls
        Projects: Projects
    }
    interface IAppState with
        member this.Engines = this.Engines
        member this.EngineInstalls = this.EngineInstalls
        member this.Projects = this.Projects

and public Engines = EngineOnline Set
and public EngineInstalls = EngineInstall ActiveSet
and public Projects = Project Set
