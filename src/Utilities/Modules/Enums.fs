module public App.Utilities.Enums

open System

/// Creates a collection of all enum values for given enum.
let inline public iterate<'a when 'a:>Enum>() =
    Enum.GetValues(typedefof<'a>) :?> 'a[]

/// Tries to create an enum value from a string. Might fail.
let inline public tryParse<'a when 'a:>Enum> txt =
    try
        Enum.Parse(typedefof<'a>, txt, true) :?> 'a |> Some
    with | _ ->
        None
