module public FSharpPlus.Enums

open System

/// Creates a collection of all enum values for given enum.
let inline public iterate<'a when 'a:>Enum>() =
    match Enum.GetValues(typedefof<'a>) |> trycast<'a[]> with
    | Some x -> x
    | None -> [||]

/// Tries to create an enum value from a string. Might fail.
let inline public tryParse<'a when 'a:>Enum and 'a: struct and 'a: (new: unit -> 'a)> (txt: string) =
    try
        Enum.Parse<'a>(txt, true) |> Some
    with | _ ->
        None
