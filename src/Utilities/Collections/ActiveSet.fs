namespace FSharpPlus

open System.Collections
open System.Collections.Generic

/// An immutable Set with one invariant: If the set is not empty, an active object within
/// that set is always defined, an empty set does not have an active object.
type public ActiveSet<'a when 'a: comparison> = private {
    set: Set<'a>
    active: 'a option
} with

    /// The underlying immutable set
    member public this.Set = this.set

    /// The currently active object, if the set is not empty
    member public this.Active = this.active

    /// Creates new active set with given element added to it
    member public this.Add elt =
        let newActive = if this.active.IsSome then this.active else Some elt
        let newSet = this.set |> Set.add elt
        {set = newSet; active = newActive}

    /// Creates new active set with given element removed from it. If the element does not
    /// exist, the set remains the same.
    member public this.Remove elt =
        let newSet = this.set |> Set.remove elt
        let newActive =
            if newSet.IsEmpty then None
            else if newSet.Contains(this.active.Value) then Some this.active.Value
            else newSet |> Set.maxElement |> Some
        {set = newSet; active = newActive}

    /// Check if this set contains given element
    member public this.Contains elt =
        this.set.Contains(elt)

    /// Creates new active set with given object as the active object. Fails if the
    /// new active object is not contained in the set.
    member public this.SetActive elt =
        if this.set.Contains(elt) then Some {this with active = Some elt} else None

    interface IReadOnlyCollection<'a> with
        member this.Count = this.set.Count
        member this.GetEnumerator(): IEnumerator<'a> = (this.set:>IEnumerable<'a>).GetEnumerator()
        member this.GetEnumerator(): IEnumerator = (this.set:>IEnumerable).GetEnumerator()

/// Functions for a Set with one invariant: If the set is not empty, an active object within
/// that set is always defined, an empty set does not have an active object.
module public ActiveSet =

    /// Creates a new, empty active set
    let public createEmpty<'a when 'a: comparison> () =
        {set = Set<'a> []; active = None}

    /// Creates an active set from given sequence. The active object must be found in
    /// the sequence, or this function will fail.
    let public createFrom seq active =
        let newSet = Set seq
        match newSet |> Set.contains active with
        | true -> Some {set = newSet |> Set; active = Some active}
        | false -> None

    /// Creates new active set with given element added to it
    let public add elt (activeSet: 'a ActiveSet) = activeSet.Add elt

    /// Creates new active set with given element removed from it. If the element does not
    /// exist, the set remains the same.
    let public remove elt (activeSet: 'a ActiveSet) = activeSet.Remove elt

    /// Check if this set contains given element
    let public contains elt (activeSet: 'a ActiveSet) = activeSet.Contains elt

    /// Creates new active set with given object as the active object. Fails if the
    /// new active object is not contained in the set.
    let public setActive elt (activeSet: 'a ActiveSet) = activeSet.SetActive elt
