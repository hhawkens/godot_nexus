namespace App.Core.Domain

open App.Core.Domain
open App.Utilities

type public IAppState =
    abstract Engines: EnginesOnline
    abstract EngineInstalls: EngineInstalls
    abstract Projects: Projects

and public AppState =
    {
        EnginesOnline: EnginesOnline
        EngineInstalls: EngineInstalls
        Projects: Projects
    }
    interface IAppState with
        member this.Engines = this.EnginesOnline
        member this.EngineInstalls = this.EngineInstalls
        member this.Projects = this.Projects

and public EnginesOnline = EngineOnline Set
and public EngineInstalls = EngineInstall ActiveSet
and public Projects = Project Set
