[<AutoOpen>]
module public App.Utilities.MonadExtensions

type public Result<'s, 'e> with

    /// Check if result object is success.
    member this.IsOk =
        match this with
        | Ok _ -> true
        | Error _ -> false

    /// Check if result object is error.
    member this.IsError =
        not this.IsOk

    /// Converts Result to Option type. A potential error object is discarded.
    member this.ToOption =
        match this with
        | Ok ok -> Some ok
        | Error _ -> None
