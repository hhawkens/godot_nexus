[<AutoOpen>]
module public FSharpPlus.FSharpPlusExtensions

/// Returns all elements of collection a that are not contained in collection b
/// (using given equality function).
let public difference<'a, 'b> equalityCheck (a: seq<'a>) (b: seq<'b>) =
    a |> filter (fun elt -> b |> exists (equalityCheck elt) |> not)
