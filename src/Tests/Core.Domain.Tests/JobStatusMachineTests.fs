namespace App.Core.Domain.Tests

open App.Core.Domain
open App.Utilities

type private TestJobStatusMachine = JobStatusMachine<int, string>
type private TestJobEndStatus = JobEndStatus<int, string>

[<AutoOpen>]
module private Common =
    let internal origin = JobStatusMachine.New<int, string> ()


module public JobStatusMachineTests =

    open NUnit.Framework

    [<Test>]
    let public ``New Status Machine Has Correct States`` () =
        Assert.That(origin.Status, Is.EqualTo(Waiting))
        Assert.That(origin.EndStatus, Is.EqualTo(TestJobEndStatus.NotEnded))


module public JobStatusMachinePropertyBasedTests =

    open FsCheck
    open FsCheck.NUnit

    type public PercentGen() =
        static member Generate() = Percent.FromInt 0uy |> unwrap |> Gen.constant |> Arb.fromGen

    [<Property (Arbitrary = [|typeof<PercentGen>|]) >]
    let public ``All Transitions From Running Have Valid Invariants`` initStatus newStatus =
        initStatus <> Ended ==> lazy
            let machineMaybe = origin |> JobStatusMachine<_,_>.Transition initStatus |> unwrap
            let machineMaybe = machineMaybe |> JobStatusMachine<_,_>.Transition newStatus
            match newStatus, machineMaybe with
            | Ended, None -> true
            | Ended, _ -> false
            | _, None -> false
            | s, Some m ->
                m.Status = s

    [<Property (Arbitrary = [|typeof<PercentGen>|]) >]
    let public ``All Conclusions From Running Have Valid Invariants`` initStatus (conclusion: TestJobEndStatus) =
        initStatus <> Ended ==> lazy
            let machineMaybe = origin |> JobStatusMachine<_,_>.Transition initStatus |> unwrap
            let machineMaybe = machineMaybe |> JobStatusMachine<_,_>.Conclude conclusion
            match conclusion, machineMaybe with
            | NotEnded, None -> true
            | NotEnded, _ -> false
            | _, None -> false
            | es, Some m ->
                m.EndStatus = es

    [<Property (Arbitrary = [|typeof<PercentGen>|]) >]
    let public ``All Transitions From Ended Fail`` status =
        let endedMachine = origin |> JobStatusMachine<_,_>.Conclude TestJobEndStatus.Aborted |> unwrap
        let machineMaybe = endedMachine |> JobStatusMachine<_,_>.Transition status
        machineMaybe.IsNone

    [<Property (Arbitrary = [|typeof<PercentGen>|]) >]
    let public ``All Conclusions From Ended Fail`` (conclusion: TestJobEndStatus) =
        let endedMachine = origin |> JobStatusMachine<_,_>.Conclude TestJobEndStatus.Aborted |> unwrap
        let machineMaybe = endedMachine |> JobStatusMachine<_, _>.Conclude conclusion
        machineMaybe.IsNone
