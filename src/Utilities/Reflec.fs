/// Utility functions using reflection.
/// Can circumvent language checks and limits, so use with care (e.g. for testing).
module public App.Utilities.Reflec

open System.Reflection
open Microsoft.FSharp.Reflection

/// Creates an instance of a (possibly private) record of given type through reflection.
/// Pass parameters needed for record creation. Throws on failure.
let public makeRecord<'a> parameters =
    (FSharpValue.MakeRecord (typedefof<'a>, parameters, BindingFlags.NonPublic)):?>'a
