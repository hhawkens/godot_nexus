namespace App.Utilities

[<AutoOpen>]
module public MaybeBuilder =

    [<Struct>]
    type public T =
        member _.Return(x) = Some x

        member _.ReturnFrom(x) = x

        member _.Bind(m, f) = Option.bind f m

        member _.Combine(a, b) = match a with | Some _ -> a | None -> b

        member _.Delay(f) = f()

    let public maybe = T()


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
