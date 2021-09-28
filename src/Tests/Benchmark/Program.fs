module public Benchmark.ProgramEntry

#if RELEASE

open BenchmarkDotNet.Running

[<EntryPoint>]
let main _args = // Uncomment benchmark you wish to run
    // BenchmarkRunner.Run typeof<LensComparison> |> ignore
    0

#else

[<EntryPoint>]
let main _args =
    printfn "Please run benchmark in RELEASE mode only!"
    1

#endif
