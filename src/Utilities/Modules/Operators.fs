namespace App.Utilities

open FSharpPlus
open Microsoft.FSharp.Quotations.Patterns

[<AutoOpen>]
module public Operators =

    /// Used to debug piped values. Value will be printed + can be stopped via Debugger.
    let public debug x =
        printf $"Debug: {x}"
        x

    /// Provides a nicely formatted string for sequences.
    let public formatSeq (sq: 'a seq) =
        sq
        |> fold (fun state e -> state + $"{e}\n") ""
        |> String.trimEnd "\n"

    /// Flattens a nested sequence into a regular one.
    let public flatten (nestedSeq: seq<'s> when 's:>seq<'a>) =
        seq { for sequence in nestedSeq do yield! sequence }

    /// Tries to cast any object to given Type (up or down).
    let public trycast<'a when 'a: not struct> (obj: obj) =
       match obj with
       | :? 'a as a -> Some a
       | _ -> None

    /// Helps creating a thread-safe wrapper function. Example:
    /// (I know F# has no (++) operator, but you get the idea)
    ///
    /// let mut = 0
    /// let threadSafe = threadSafeFactory ()
    /// Async.Parallel [async { threadSafe (fun _ -> mut++) }; async { threadSafe (fun _ -> mut++) }]
    let public threadSafeFactory () =
        let threadSafeFactoryInternal guard action =
            lock guard action
        threadSafeFactoryInternal (obj())

    /// Starts a sequence of async tasks in parallel pool threads. Example:
    ///
    /// async {
    ///     let! tasks = [calcAsync 1; calcAsync 2; calcAsync 3;] |> startParallel
    ///     ... (do other stuff)
    ///     let! results = tasks }
    let public startParallel task = task |> Async.Parallel |> Async.StartChild

    /// Converts the output of an exception throwing function to a result type.
    let public exnToResult func =
        try
            func() |> Ok
        with | exn ->
             exn.Message |> Error

    /// Converts the output of an exception throwing async-function to an async result type.
    let public exnToResultAsync func = async {
        try
            let! result = func ()
            return Ok result
        with | exn ->
             return exn.Message |> Error
    }

    /// Placeholder function that does nothing and takes n parameters. Has configurable return value.
    let public nothing ret = ret
    let public nothing1 ret _ = ret
    let public nothing2 ret _ _ = ret
    let public nothing3 ret _ _ _ = ret
    let public nothing4 ret _ _ _ _ = ret

    /// Deterministically hashes a string into a byte. Uniqueness is diminished compared to a full int.
    let public stringToByte (str: string) =
        str
        |> String.toSeq
        |> fold (fun hash c -> (hash * 7uy) + (byte c)) 7uy

    /// Used to retrieve the type of a module (inside the module itself),
    /// which by "normal" means like typedefof is not possible. Usage example:
    /// module SomeName =
    ///     let rec private selfType = moduleType <@ selfType @>
    let moduleType = function
    | PropertyGet (_, propertyInfo, _) -> propertyInfo.DeclaringType
    | _ -> failwith "Expression is no property."


[<AutoOpen>]
type public Operators =

    /// Forcefully unpacks Result type, assuming it is a success. Throws exception on failure.
    static member public unwrap result =
        match result with
        | Ok ok -> ok
        | Error err -> failwith $"Result unwrap failed: {err}"

    /// Forcefully unpacks Result type, assuming it is an error. Throws exception on success.
    static member public unwrapError result =
        match result with
        | Ok ok -> failwith $"Result error-unwrap failed, was success object: {ok}"
        | Error err -> err

    /// Forcefully unpacks Option type, assuming it is "Some". Throws exception on failure.
    static member public unwrap opt =
        match opt with
        | Some s -> s
        | None -> failwith "Option unwrap failed"

    /// Throws if given assertion fails. Can be used to uphold asserted app invariants.
    static member public assertThat errorMsg assertion =
        match assertion with | true -> () | false -> failwith errorMsg
