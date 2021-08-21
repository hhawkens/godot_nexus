[<AutoOpen>]
module public App.Shell.State.IAppStateControllerExtensions

type public IAppStateController with

    member this.HasEngineToOpenProjectsWith = not this.State.EngineInstalls.Set.IsEmpty
