namespace App.Utilities.Tests

open App.Utilities
open FSharpPlus

module public ActiveSetTests =

    open NUnit.Framework

    [<Literal>]
    let private ActiveElement = 2;
    let private emptyActiveSet = ActiveSet.createEmpty<string> ()
    let private testActiveSet = ActiveSet.createFrom (seq {1; 2; 3; 4}) ActiveElement |> unwrap

    [<Test>]
    let public ``Create Empty Set With No Content And No Active Object`` () =
        let set = ActiveSet.createEmpty ()
        Assert.That(set.Set, Is.EquivalentTo(Set []))
        Assert.That(set.Active, Is.EqualTo(None))

    [<Test>]
    let public ``Can Access Set Backend`` () =
        Assert.That(testActiveSet.Set, Is.EquivalentTo(Set [1;2;3;4]))
        Assert.That(emptyActiveSet.Set, Is.EquivalentTo(Set []))

    [<Test>]
    let public ``Can Access Active Object`` () =
        Assert.That(testActiveSet.Active, Is.EqualTo(Some ActiveElement))
        Assert.That(emptyActiveSet.Active, Is.EqualTo(None))

    [<Test>]
    let public ``Element Can Be Added`` () =
        let added1 = testActiveSet |> ActiveSet.add 99
        let added2 = testActiveSet.Add 99
        Assert.That(added1, Is.EquivalentTo([1; 2; 3; 4; 99]))
        Assert.That(added2, Is.EquivalentTo([1; 2; 3; 4; 99]))

    [<Test>]
    let public ``Element Can Be Removed`` () =
        let added1 = testActiveSet |> ActiveSet.remove 2
        let added2 = testActiveSet.Remove 2
        Assert.That(added1, Is.EquivalentTo([1; 3; 4]))
        Assert.That(added2, Is.EquivalentTo([1; 3; 4]))

    [<Test>]
    let public ``If Active Element Is Removed The Max Object Is The New Active`` () =
        let added1 = testActiveSet |> ActiveSet.remove ActiveElement
        Assert.That(added1.Active, Is.EqualTo(Some 4))

    [<Test>]
    let public ``Removing Element That Does Not Exist Does Nothing`` () =
        let activeSet = testActiveSet |> ActiveSet.remove 777
        Assert.That(activeSet, Is.EqualTo(testActiveSet))

    [<Test>]
    let public ``If All Elements Are Removed Active Object Is None`` () =
        let set = testActiveSet |> ActiveSet.remove 1
        let set = set |> ActiveSet.remove 2
        let set = set |> ActiveSet.remove 3
        let set = set |> ActiveSet.remove 4
        Assert.That(set.Active, Is.EqualTo(None))

    [<Test>]
    let public ``Can Check If Set Contains Element`` () =
        Assert.That(testActiveSet.Contains(4), Is.True)
        Assert.That(testActiveSet.Contains(5), Is.False)
        Assert.That(testActiveSet |> ActiveSet.contains 4, Is.True)
        Assert.That(testActiveSet |> ActiveSet.contains 5, Is.False)

    [<Test>]
    let public ``Set Active With Existing Object Sets New Active`` () =
        Assert.That((testActiveSet.SetActive(1) |> unwrap).Active, Is.EqualTo(Some 1))
        Assert.That((testActiveSet |> ActiveSet.setActive 1 |> unwrap).Active, Is.EqualTo(Some 1))

    [<Test>]
    let public ``Set Active With Invalid Active Returns None`` () =
        Assert.That(testActiveSet.SetActive(99), Is.EqualTo(None))
        Assert.That(testActiveSet |> ActiveSet.setActive 99, Is.EqualTo(None))

module public ActiveSetPropertyBasedTests =

    open FsCheck.NUnit

    let private checkInvariants (activeSet: ActiveSet<'a>) =
        match activeSet.Set.IsEmpty with
        | true -> activeSet.Active = None
        | false ->
            activeSet.Active.IsSome &&
            activeSet.Set.Contains(activeSet.Active.Value)

    [<Property>]
    let public ``Check Invariants With All Params Randomized`` (set: Set<int64>) (active: int64) =
        match ActiveSet.createFrom set active with
        | Some activeSet -> checkInvariants activeSet
        | None -> set.Contains(active) |> not

    [<Property>]
    let public ``Check Invariants With Valid Active Object`` (inputList: string list) =
        match inputList.IsEmpty with
        | true -> true
        | false ->
            let rand = System.Random(123)
            let active = inputList.[rand.Next(0, inputList.Length - 1)]
            let activeSet = ActiveSet.createFrom (Set inputList) active |> unwrap
            checkInvariants activeSet

    [<Property>]
    let public ``Check Invariants After Adding`` (addedNums: int list) =
        let mutable activeSet = ActiveSet.createEmpty ()
        let mutable success = true
        for added in addedNums do
            activeSet <- activeSet |> ActiveSet.add added
            let result = checkInvariants activeSet
            success <- success && result
        success

    [<Property>]
    let public ``Check Invariants After Removing`` (inputList: float list) =
        match inputList with
        | [] -> true
        | list ->
            let activeSet = ActiveSet.createFrom (Set list) list.[0] |> unwrap
            let result =
                activeSet
                |> fold (fun (set, success) curr ->
                    let removedSet = set |> ActiveSet.remove curr
                    let success = success && checkInvariants removedSet
                    (removedSet, success) ) (activeSet, true)
            result |> snd

    [<Property>]
    let public ``Check Invariants After Setting Active`` (inputList: int16 list) =
        match inputList with
        | [] -> true
        | list ->
            let activeSet = ActiveSet.createFrom (Set list) list.[0] |> unwrap
            let result =
                activeSet
                |> fold (fun (set, success) curr ->
                    let setWithNewActive = set |> ActiveSet.setActive curr |> unwrap
                    let success = success && checkInvariants setWithNewActive
                    (setWithNewActive, success) ) (activeSet, true)
            result |> snd
