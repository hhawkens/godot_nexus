namespace App.Utilities

[<Struct>]
/// Flag indicating whether some value was set or not.
type public SetState =
    | WasSet
    | NotSet

/// Wraps a value that can be changed only once after initialization.
type public SetOnce<'a>(initValue: 'a) =
    let mutable currentValue = initValue
    let mutable wasSet = NotSet

    override this.ToString() = $"{nameof SetOnce}[{currentValue}]"

    /// The currently set value.
    member this.Value = currentValue

    /// Sets a new value, works once only.
    member public this.Set newValue =
        this.SetChecked newValue |> ignore

    /// Sets a new value, only works once. Returns an indicator whether the value was set.
    member public this.SetChecked newValue =
        match wasSet with
        | WasSet -> NotSet
        | NotSet ->
            currentValue <- newValue
            wasSet <- WasSet
            WasSet

    /// Sets a new value, only works once. Throws if set multiple times.
    member public this.SetOrFail newValue =
        match this.SetChecked newValue with
        | WasSet -> ()
        | NotSet -> failwith $"Tried to set value more than once for {this}"
