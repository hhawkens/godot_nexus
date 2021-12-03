namespace FSharpPlus

/// Represents a percent value, clamped between 0.0 and 1.0.
type public Percent = private Percent of float with

    /// The numeric value of the percentage, in floating point. E.g. 0.1, which is 10%.
    member public this.Value =
        let (Percent p) = this
        p

    /// Creates a percentage from an integer. Has to be between 0 and 100.
    static member public FromInt = function
        | i when i >= 0uy && i <= 100uy ->
            i |> float |> (*)0.01 |> Percent |> Some
        | _ -> None

    /// Creates a percentage from a float. Has to be between 0.0 and 1.0.
    static member public FromFloat = function
        | f when f >= 0.0 && f <= 1.0 ->
            Percent f |> Some
        | _ -> None

    /// String representation.
    override this.ToString() =
        let readableValue = this.Value * 100.0
        $"{readableValue:F1} %%"
