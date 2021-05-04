module public App.Utilities.Compare

open FSharpPlus

/// Returns all differences (on public properties) between two objects of the same Type
let public allPropertyDiffs (a: 't) (b: 't) =
    typedefof<'t>.GetProperties()
    |> choose (fun prop ->
       match not <| prop.GetValue(a).Equals(prop.GetValue(b)) with
       | true -> Some prop
       | false -> None)
