module public App.Benchmark.ProgramEntry

#if RELEASE

[<EntryPoint>]
let main _args = // Uncomment benchmark you wish to run
    // BenchmarkDotNet.Running.BenchmarkRunner.Run typeof<LensComparison> |> ignore
    0

#else

[<EntryPoint>]
let main _args =
    printfn "Please run benchmark in RELEASE mode only!"
    1

#endif
