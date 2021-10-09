namespace App.Utilities

/// Usage Example:
///
/// type Human  = {Name:string}
/// and  Seat   = {Number:int; UsedBy:Human option}
/// and  Bus    = {DriverSeat:Seat}
/// and  Company= {Name:string;SchoolBus:Bus}
///
/// let BusDriver = {Human.Name="BusDriver A."}
/// let SchoolBus = {DriverSeat={Seat.Number=1;UsedBy=Some BusDriver}}
/// let BusCompany= {SchoolBus=SchoolBus; Name="EasyBus"}
///
///
/// do printfn "company: %A" BusCompany
/// company: {Name = "EasyBus"; SchoolBus = {DriverSeat = {
///  Number = 1; UsedBy = Some {Name = "BusDriver A.";};};};}
///
///do printfn "swapDriver: %A" <| Lenses.With <@ BusCompany.SchoolBus.DriverSeat.UsedBy @> (Some {Human.Name="BusDriver Z."})
/// swapDriver: {Name = "EasyBus"; SchoolBus = {DriverSeat = {
///  Number = 1; UsedBy = Some {Name = "BusDriver Z.";};};};}
///
///do printfn "rmDriver: %A" <| Lenses.With <@ BusCompany.SchoolBus.DriverSeat.UsedBy @> None
/// rmDriver: {Name = "EasyBus"; SchoolBus = {DriverSeat = {
///  Number = 1; UsedBy = null;};};}
///
/// do printfn "mvSeat: %A" <| Lenses.With <@ BusCompany.SchoolBus.DriverSeat.Number @> 12345
/// mvSeat: {Name = "EasyBus"; SchoolBus = {DriverSeat = {
///  Number = 12345; UsedBy = Some {Name = "BusDriver A.";};};};}
[<AutoOpen>]
module public Lenses =

    open FSharp.Quotations
    open FSharp.Quotations.Patterns
    open FSharp.Quotations.Evaluator
    open Microsoft.FSharp.Reflection
    open System.Reflection

    let rec private selfType = lazy moduleType <@ selfType @>

    module internal Record =

        let private eval = QuotationEvaluator.EvaluateUntyped

        let private fields = FSharpType.GetRecordFields

        let private type' (p:PropertyInfo) = p.DeclaringType

        let private valuesOr p v o =
            type' p |> fields
            |> Array.map (fun x -> if x = p then v else x.GetValue(o))

        let private make t xs = FSharpValue.MakeRecord(t,xs,false)

        let private with' p v o = valuesOr p v o |> make (type' p)

        let rec internal update<'a,'b> = function
            | PropertyGet(None,_,[]),v -> v
            | ValueWithName _,v -> v
            | PropertyGet(Some(PropertyGet _ as pg),p,[]),v -> update(pg,(with' p v (eval pg)))
            | PropertyGet(Some(ValueWithName _ as pg),p,[]),v -> update(pg,(with' p v (eval pg)))
            | _ -> failwith $"Unexpected failure in {selfType.Value.Name}"

    let private unsafeLens (e:Expr<'a>) (v:'a) (_: 'b) = Record.update(e.Raw,v) :?> 'b

    /// See type "Lenses" for more detailed information. Quick usage example:
    /// let newRecord = oldRecord |> withLens <@ oldRecord.A.B.C @> "New Value of C"
    /// WARNING: This method is at least 10000x slower than regular record updating, use cautiously!
    /// Also, it only works if all members of a record are public.
    let public lens expr value origin =
        (fun _ -> unsafeLens expr value origin)
        |> exnToResult
