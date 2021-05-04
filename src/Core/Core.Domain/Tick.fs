namespace App.Core.Domain

open System

/// Represents a point in time, relative to the last tick.
/// One tick equals around 100 ms.
[<Struct>]
type public Tick = {
    TimeStamp: uint64
} with
    static member TickDuration = TimeSpan.FromMilliseconds 100.0

/// Abstract number of ticks (one tick equals around 100 ms).
[<Struct>]
type public Ticks(count: uint) =
    member this.Count = count
