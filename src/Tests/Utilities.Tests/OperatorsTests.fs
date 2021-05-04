module public App.Utilities.Tests.OperatorsTests

open System
open System.Threading
open NUnit.Framework
open App.Utilities

type public I = interface end
type public A () = class interface I end
type public B () = class inherit A() end

[<Test>]
let public ``Unwrap Of Some Propagates Object`` () =
    Assert.That(Some 100 |> unwrap, Is.EqualTo(100))

[<Test>]
let public ``Unwrap Of None Throws`` () =
    Assert.That(Action (fun () -> None |> unwrap), Throws.Exception)

[<Test>]
let public ``Unwrap Of Ok Propagates Object`` () =
    Assert.That(Ok "Mkay" |> unwrap, Is.EqualTo("Mkay"))

[<Test>]
let public ``Unwrap Of Error Throws`` () =
    Assert.That(Action (fun () -> Error "not good" |> unwrap), Throws.Exception)

[<Test>]
let public ``UnwrapError Of Error Returns Error Object`` () =
    Assert.That(Error "error info" |> unwrapError, Is.EqualTo("error info"))

[<Test>]
let public ``UnwrapError Of Success Throws`` () =
    Assert.That(Action (fun () -> Ok () |> unwrapError), Throws.Exception)

[<Test>]
let public ``FormatSeq Prints All Elements Of A Sequence`` () =
    let sequence =  seq {5; 6; 7}
    let expectedText = "5\n6\n7"
    Assert.That(formatSeq sequence, Is.EqualTo(expectedText))

[<Test>]
let public ``FormatSeq Prints Nothing On An Empty Sequence`` () =
    Assert.That(formatSeq [], Is.EqualTo(""))

[<Test>]
let public ``Flatten Of Empty Returns Empty`` () =
    let empty = []
    let flat = flatten empty
    Assert.That(flat, Is.EquivalentTo([]))

[<Test>]
let public ``Flatten Can Unwind A List Of Lists`` () =
    let listOfLists = [["1";"2";"3"];["4";"5"];["6"]]
    let flat = flatten listOfLists
    Assert.That(flat, Is.EquivalentTo(["1";"2";"3";"4";"5";"6"]))

[<Test>]
let public ``Flatten Can Unwind An Array Of Sets`` () =
    let arrayOfSets = [|["1";"2";"3"] |> Set; ["4";"5"] |> Set; ["6"] |> Set|]
    let flat = flatten arrayOfSets
    Assert.That(flat, Is.EquivalentTo(["1";"2";"3";"4";"5";"6"]))

[<Test>]
let public ``Casting Only Works When Type Is Inherited`` () =
    let a = A ()
    let b = B ()
    Assert.That(trycast<A> a |> unwrap, Is.EqualTo(a))
    Assert.That(trycast<B> a, Is.EqualTo(None))
    Assert.That(trycast<A> b |> unwrap, Is.AssignableTo<A>())
    Assert.That(trycast<B> b |> unwrap, Is.EqualTo(b))
    Assert.That(trycast<I> b |> unwrap, Is.AssignableTo<I>())
    Assert.That(trycast<string> b, Is.EqualTo(None))

[<Test>]
let public ``Thread Safe Factory Produces Locking Functionality`` () =
    let threadSafe = threadSafeFactory ()
    let mutable nums = ResizeArray<uint64>()
    let f1 () = for n in 1UL..10_000UL do threadSafe (fun _ -> nums.Add(n))
    let f2 () = for n in 10_001UL..20_000UL do threadSafe (fun _ -> nums.Add(n))
    let f3 () = for n in 20_001UL..30_000UL do threadSafe (fun _ -> nums.Add(n))

    Async.Parallel [ async { f1 () }; async { f2 () }; async { f3 () } ]
    |> Async.RunSynchronously
    |> ignore

    // If the functions were not properly locked the number would be way lower
    Assert.That(nums.Count, Is.EqualTo(30_000))

[<Test>]
let public ``Thread Safe Factory Uses Different Locks Each Time It Is Called`` () =
    let threadSafe = threadSafeFactory ()
    let threadSafe2 = threadSafeFactory ()
    let mutable x = 0
    let increment () = x <- x + 1
    threadSafe (fun _ ->
        increment ()
        Async.Start (async { threadSafe2 increment })
        while x <> 2 do Thread.Sleep 10 // Check for deadlock
        )
    Assert.That(x, Is.EqualTo(2))

[<Test>]
let public ``Start Parallel Runs In Pool Threads And Returns Correct Values`` () =
    let mutable count = 0
    let increment x = async {
        for _ in 0..1000 do count <- count + 1
        return x
    }
    let result =
        [increment 1; increment 2; increment 3; increment 4; increment 5]
        |> startParallel
        |> Async.RunSynchronously
        |> Async.RunSynchronously

    Assert.That(result, Is.EquivalentTo([1;2;3;4;5]))
    Assert.That(count, Is.LessThan(5000)) // Result would be over 5000 if run sequentially

[<Test>]
let public ``Exception Message Is Converted To Result Type`` () =
    let result = exnToResult (fun () -> failwith "Something bad happened") |> unwrapError
    Assert.That(result, Is.EqualTo("Something bad happened"))

[<Test>]
let public ``Non-Exception Is Passed Untouched`` () =
    let result = exnToResult (fun () -> 1.11) |> unwrap
    Assert.That(result, Is.EqualTo(1.11))

[<Test>]
let public ``Async Exception Message Is Converted To Result Type`` () =
    let result =
        exnToResultAsync (fun () -> async { failwith "Something bad happened" })
        |> Async.RunSynchronously
        |> unwrapError
    Assert.That(result, Is.EqualTo("Something bad happened"))

[<Test>]
let public ``Async Non-Exception Is Passed Untouched`` () =
    let result =
        exnToResultAsync (fun () -> async { return 1.11 })
        |> Async.RunSynchronously
        |> unwrap
    Assert.That(result, Is.EqualTo(1.11))

[<Test>]
let public ``String To Byte Hashing Is Relatively Unique`` () =
    let texts = [|"a";"ab";"abc";"cba";"Down";"Up";"UpDown";"up";"down";"test";"tester";"huh?";"Wreck"|]
    let mutable hashes = Set.empty
    for txt in texts do
        let byteHash = stringToByte txt
        Assert.That(hashes.Contains byteHash, Is.False)
        hashes <- hashes.Add byteHash
