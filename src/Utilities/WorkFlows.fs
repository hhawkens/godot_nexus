namespace App.Utilities

[<AutoOpen>]
module public MaybeBuilder =

    [<Struct>]
    type public T =
        member _.Return(x) = Some x

        member _.ReturnFrom(x) = x

        member _.Combine(a, b) = match a with | Some _ -> a | None -> b

        member _.Delay(f) = f()

    let public maybe = T()
