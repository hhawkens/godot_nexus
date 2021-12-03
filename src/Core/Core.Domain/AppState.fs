namespace App.Core.Domain

open App.Core.Domain
open FSharpPlus

type public IAppState =
    abstract EnginesOnline: EnginesOnline
    abstract EngineInstalls: EngineInstalls
    abstract Projects: Projects


and public AppState =
    {
        EnginesOnline: EnginesOnline
        EngineInstalls: EngineInstalls
        Projects: Projects
    }

    interface IAppState with

        member this.EnginesOnline =
            (this.EnginesOnline, this.EngineInstalls)
            ||> difference (fun a b -> a.Data.Id = b.Data.Id)
            |> Set

        member this.EngineInstalls = this.EngineInstalls

        member this.Projects = this.Projects


and public EnginesOnline = EngineOnline Set
and public EngineInstalls = EngineInstall ActiveSet
and public Projects = Project Set
