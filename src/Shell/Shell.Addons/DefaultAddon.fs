namespace App.Shell.Addons

open System
open App.Shell.State
open FSharpPlus

type public DefaultAddon =
    {
        Id: string
        BeforeInitializeTask: unit AddonTask option
        AfterInitializeTask: IAppStateController AddonTask option
        TickTask: TickTask option
    }

    override this.Equals(obj) =
        match obj |> trycast<DefaultAddon> with
        | Some other -> this.Id.Equals(other.Id)
        | None -> false

    override this.GetHashCode() = this.Id.GetHashCode()

    override this.ToString() = $"ADDON [{this.Id}]"

    interface IComparable with
        member this.CompareTo(obj) =
            match obj |> trycast<DefaultAddon> with
            | Some other -> this.Id.CompareTo(other.Id)
            | None -> -1

    interface IAddon with
        member this.Id = this.Id
        member this.BeforeInitializeTask = this.BeforeInitializeTask
        member this.AfterInitializeTask = this.AfterInitializeTask
        member this.TickTask = this.TickTask
