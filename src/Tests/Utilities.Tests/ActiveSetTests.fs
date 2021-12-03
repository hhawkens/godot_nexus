namespace FSharpPlus.Tests

open FSharpPlus

module public ActiveSetTests =

    open NUnit.Framework

    [<Literal>]
    let private ActiveElement = 2;
    let private emptySut = ActiveSet.createEmpty<string> ()
    let private sut = ActiveSet.createFrom (seq {1; 2; 3; 4}) ActiveElement |> unwrap

    [<Test>]
    let public ``Create Empty Set With No Content And No Active Object`` () =
        let set = ActiveSet.createEmpty ()
        Assert.That(set.Set, Is.EquivalentTo(Set []))
        Assert.That(set.Active, Is.EqualTo(None))

    [<Test>]
    let public ``Can Access Set Backend`` () =
        Assert.That(sut.Set, Is.EquivalentTo(Set [1;2;3;4]))
        Assert.That(emptySut.Set, Is.EquivalentTo(Set []))

    [<Test>]
    let public ``Can Access Active Object`` () =
        Assert.That(sut.Active, Is.EqualTo(Some ActiveElement))
        Assert.That(emptySut.Active, Is.EqualTo(None))

    [<Test>]
    let public ``Element Can Be Added`` () =
        let added1 = sut |> ActiveSet.add 99
        let added2 = sut.Add 99
        Assert.That(added1, Is.EquivalentTo([1; 2; 3; 4; 99]))
        Assert.That(added2, Is.EquivalentTo([1; 2; 3; 4; 99]))

    [<Test>]
    let public ``Element Can Be Removed`` () =
        let added1 = sut |> ActiveSet.remove 2
        let added2 = sut.Remove 2
        Assert.That(added1, Is.EquivalentTo([1; 3; 4]))
        Assert.That(added2, Is.EquivalentTo([1; 3; 4]))

    [<Test>]
    let public ``If Active Element Is Removed The Max Object Is The New Active`` () =
        let added1 = sut |> ActiveSet.remove ActiveElement
        Assert.That(added1.Active, Is.EqualTo(Some 4))

    [<Test>]
    let public ``Removing Element That Does Not Exist Does Nothing`` () =
        let activeSet = sut |> ActiveSet.remove 777
        Assert.That(activeSet, Is.EqualTo(sut))

    [<Test>]
    let public ``If All Elements Are Removed Active Object Is None`` () =
        let set = sut |> ActiveSet.remove 1
        let set = set |> ActiveSet.remove 2
        let set = set |> ActiveSet.remove 3
        let set = set |> ActiveSet.remove 4
        Assert.That(set.Active, Is.EqualTo(None))

    [<Test>]
    let public ``Can Check If Set Contains Element`` () =
        Assert.That(sut.Contains(4), Is.True)
        Assert.That(sut.Contains(5), Is.False)
        Assert.That(sut |> ActiveSet.contains 4, Is.True)
        Assert.That(sut |> ActiveSet.contains 5, Is.False)

    [<Test>]
    let public ``Set Active With Existing Object Sets New Active`` () =
        Assert.That((sut.SetActive(1) |> unwrap).Active, Is.EqualTo(Some 1))
        Assert.That((sut |> ActiveSet.setActive 1 |> unwrap).Active, Is.EqualTo(Some 1))

    [<Test>]
    let public ``Set Active With Invalid Active Returns None`` () =
        Assert.That(sut.SetActive(99), Is.EqualTo(None))
        Assert.That(sut |> ActiveSet.setActive 99, Is.EqualTo(None))

module public ActiveSetPropertyBasedTests =

    open FsCheck.NUnit

    let private checkInvariants (sut: ActiveSet<'a>) =
        match sut.Set.IsEmpty with
        | true -> sut.Active = None
        | false ->
            sut.Active.IsSome &&
            sut.Set.Contains(sut.Active.Value)

    [<Property>]
    let public ``Check Invariants With All Params Randomized`` (set: Set<int64>) (active: int64) =
        match ActiveSet.createFrom set active with
        | Some sut -> checkInvariants sut
        | None -> set.Contains(active) |> not

    [<Property>]
    let public ``Check Invariants With Valid Active Object`` (inputList: string list) =
        match inputList.IsEmpty with
        | true -> true
        | false ->
            let rand = System.Random(123)
            let active = inputList.[rand.Next(0, inputList.Length - 1)]
            let sut = ActiveSet.createFrom (Set inputList) active |> unwrap
            checkInvariants sut

    [<Property>]
    let public ``Check Invariants After Adding`` (addedNums: int list) =
        let mutable sut = ActiveSet.createEmpty ()
        let mutable success = true
        for added in addedNums do
            sut <- sut |> ActiveSet.add added
            let result = checkInvariants sut
            success <- success && result
        success

    [<Property>]
    let public ``Check Invariants After Removing`` (inputList: float list) =
        match inputList with
        | [] -> true
        | list ->
            let sut = ActiveSet.createFrom (Set list) list.[0] |> unwrap
            let result =
                sut
                |> fold (fun (set, success) curr ->
                    let removedSet = set |> ActiveSet.remove curr
                    let success = success && checkInvariants removedSet
                    (removedSet, success) ) (sut, true)
            result |> snd

    [<Property>]
    let public ``Check Invariants After Setting Active`` (inputList: int16 list) =
        match inputList with
        | [] -> true
        | list ->
            let sut = ActiveSet.createFrom (Set list) list.[0] |> unwrap
            let result =
                sut
                |> fold (fun (set, success) curr ->
                    let setWithNewActive = set |> ActiveSet.setActive curr |> unwrap
                    let success = success && checkInvariants setWithNewActive
                    (setWithNewActive, success) ) (sut, true)
            result |> snd
