[<AutoOpen>]
module public FSharpPlus.MonadExtensions

type public Result<'s, 'e> with

    /// Check if result object is success.
    member public this.IsOk =
        match this with
        | Ok _ -> true
        | Error _ -> false

    /// Check if result object is error.
    member public this.IsError =
        not this.IsOk

    /// Converts Result to Option type. A potential error object is discarded.
    member public this.ToOption =
        match this with
        | Ok ok -> Some ok
        | Error _ -> None

    /// Check if result object is success.
    static member public isOk (result: Result<_,_>) = result.IsOk

    /// Check if result object is error.
    static member public isError (result: Result<_,_>) = result.IsError

    /// Converts Result to Option type. A potential error object is discarded.
    static member public toOption (result: Result<_,_>) = result.ToOption


type public Option<'a> with

    /// Converts an option to a Result (using given error object in case of failure)
    static member public toError err option =
        match option with
        | Some x -> Ok x
        | None -> Error err

    /// Converts an option to a Result (using given error object in case of failure)
    member public this.ToError err = Option.toError err this
