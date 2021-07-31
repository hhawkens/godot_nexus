namespace App.Shell.Addons

open App.Core.Domain
open App.Core.State

type public AddonTask<'a> =
    | AddonSync of ('a -> unit)
    | AddonAsync of ('a -> unit Async)

type public TickTask = {
    ExecuteEvery: Ticks
    Task: (Tick * IAppStateController) AddonTask
}
