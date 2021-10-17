[<AutoOpen; System.Runtime.CompilerServices.Extension>]
module public App.Utilities.BaseTypeExtensions

open System.Text.RegularExpressions
open System.Runtime.CompilerServices

/// Prettifies pascal case text, e.g. "HelloWorld" -> "Hello World".
[<Extension>]
let public SplitPascalCase (source: string) =
    let regex = Regex(@"(?<=[a-z])([A-Z])|(?<=[A-Z])([A-Z][a-z])")
    regex.Replace(source, @" $1$2")
