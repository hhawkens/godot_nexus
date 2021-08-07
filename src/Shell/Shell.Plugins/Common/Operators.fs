[<AutoOpen>]
module internal App.Shell.Plugins.Operators

open App.Core.Domain
open App.Utilities

let generateJobId<'a when 'a:>IJob> () =
    let idVal = Rand.NextI32() |> IdVal
    let prefixSub =  typedefof<'a>.Name |> stringToByte |> IdPrefixSub
    Id.WithPrefixSub IdPrefixes.job prefixSub idVal
