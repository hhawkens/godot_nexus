namespace Benchmark

open BenchmarkDotNet.Attributes
open App.Utilities

[<MemoryDiagnoser>]
type public LensComparison () =

    [<Params (10, 1000)>]
    member val size :int = 0 with get, set

    static member val private testRecord =
        let enginesPath = {Description = "Engines Path"; DefaultValue = "/Engines"; CurrentValue = "/Engines"}
        let projectsPath = {Description = "Projects Path"; DefaultValue = "/P", "/p"; CurrentValue = "/P", "/p"}
        let theme = {Description = "Theme"; DefaultValue = TestTheme.System; CurrentValue = TestTheme.System}
        let general = {EnginesPath = enginesPath; ProjectsPath = projectsPath}
        let ui = {Theme = theme}
        {General = general; UI = ui}

    static member private ChangeRecordWithLens () =
        let record = LensComparison.testRecord
        record |> lens <@ record.General.ProjectsPath.CurrentValue @> ("aaa", "bbb") |> ignore

    static member private ChangeRecordTraditionally () =
        let record = LensComparison.testRecord
        {
            record with General = {
                    record.General with ProjectsPath = { record.General.ProjectsPath with CurrentValue = "aaa", "bbb"}
            }
        }
        |> ignore

    [<Benchmark>]
    member self.ChangeRecordWithLens() =
        for x in 0..self.size do
            LensComparison.ChangeRecordWithLens ()

    [<Benchmark>]
    member self.ChangeRecordTraditionally () =
        for x in 0..self.size do
            LensComparison.ChangeRecordTraditionally ()
