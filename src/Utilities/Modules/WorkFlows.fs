namespace App.Utilities

[<AutoOpen>]
module public ResultBuilder =

    [<Struct>]
    type public T =
        member _.Return(x) = Ok x

        member _.ReturnFrom(x) = x

        member _.Bind(m, f) = Result.bind f m

        member _.Combine(a, b) = match a with | Ok _ -> a | Error _ -> b

        member _.Delay(f) = f()

    let public result = T()
