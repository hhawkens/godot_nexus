namespace App.Core.Plugins.Tests

open System
open System.Threading
open NUnit.Framework
open App.Core.Domain
open App.Core.Plugins
open App.Utilities

module public DownloadEngineJobTests =

    let private testFileData = Reflec.makeRecord<FileData> [|"no_path"|]
    let private testCacheDir = CacheDirectory (DirectoryData.current())
    let private testError = async{ return Result<FileData, ErrorMessage>.Error "test error" }
    let private testSuccess = async{ return Result<FileData, ErrorMessage>.Ok  testFileData}
    let private testEngineData = {Version = Version(1, 2, 3); DotNetSupport = NoSupport}
    let private testUri = "https://downloads.tuxfamily.org/godotengine/3.2.1/Godot_v3.2.1-stable_x11.64.zip"
    let private testEngine = EngineOnline.New testEngineData testUri (FileSize.FromMegabytes 20.0)

    [<Test>]
    let public ``Has Unique Id With Correct Prefix`` () =
        let mutable ids = Set.empty
        let mutable prefixSubs = Set.empty

        for i in 0..100 do
            let job = DownloadEngineJob(testCacheDir, testEngine, (nothing4 testError))
            let id = (job:>IDownloadEngineJob).Id
            prefixSubs <- if prefixSubs.IsEmpty then prefixSubs.Add id.PrefixSub else prefixSubs
            Assert.That(id.Prefix, Is.EqualTo(IdPrefixes.job))
            Assert.That(ids.Contains id, Is.False)
            Assert.That(prefixSubs.Contains id.PrefixSub)
            ids <- ids.Add id

    [<Test>]
    let public ``Correct Status Change Propagated On Error`` () =
        let downloader _ _ _ (hook: DownloadProgress -> unit) =
            for _ in 0..70 do
                DownloadProgress struct{|Current = {bytes = 10UL}; Total = {bytes = 100UL}|} |> hook
            testError

        let job = DownloadEngineJob(testCacheDir, testEngine, downloader)
        let iJob = job:>IDownloadEngineJob

        let mutable states = [iJob.Status, iJob.EndStatus]
        iJob.Updated.Add (fun _ -> states <- (iJob.Status, iJob.EndStatus)::states)
        iJob.Run() |> Async.RunSynchronously

        match states with
        | (Ended, Failed err)::(Running run2, NotEnded)::(Running run1, NotEnded)::[Waiting, NotEnded] ->
            Assert.That(run1.Progress.IsNone)
            Assert.That(run2.Progress, Is.EqualTo(Some(Percent.FromFloat 0.1 |> unwrap)))
            Assert.That(err, Is.EqualTo("test error"))
        | _ -> Assert.Fail("Wrong sequence of status updates!")

    [<Test>]
    let public ``Correct Status Change Propagated On Success`` () =
        let job = DownloadEngineJob(testCacheDir, testEngine, (nothing4 testSuccess))
        let iJob = job:>IDownloadEngineJob

        let mutable states = [iJob.Status, iJob.EndStatus]
        iJob.Updated.Add (fun _ -> states <- (iJob.Status, iJob.EndStatus)::states)
        iJob.Run() |> Async.RunSynchronously
        iJob.Run() |> Async.RunSynchronously // check if can run only once

        match states with
        | (Ended, Succeeded (file, engine))::(Running _, NotEnded)::[Waiting, NotEnded] ->
            Assert.That(file.Val, Is.EqualTo(testFileData))
            Assert.That(engine, Is.EqualTo(testEngine))
        | _ -> Assert.Fail("Wrong sequence of status updates!")

    [<Test>]
    let public ``Correct Status Change Propagated On Abort`` () =
        let downloader _ _ (cancel: CancellationToken) _ =
            let mutable countdown = 300
            while countdown > 0 && not cancel.IsCancellationRequested do
                Thread.Sleep 10
                countdown <- countdown - 1
            failwith "Test is taking too long!"

        let job = DownloadEngineJob(testCacheDir, testEngine, downloader)
        let iJob = job:>IDownloadEngineJob

        let mutable states = [iJob.Status, iJob.EndStatus]
        iJob.Updated.Add (fun _ -> states <- (iJob.Status, iJob.EndStatus)::states)
        iJob.Run() |> Async.StartChild |> Async.RunSynchronously |> ignore

        Thread.Sleep 25
        iJob.Abort()
        Thread.Sleep 25

        match states with
        | (Ended, Aborted)::(Aborting, NotEnded)::(Running _, NotEnded)::[Waiting, NotEnded] ->
            Assert.Pass()
        | _ ->
            Assert.Fail("Wrong sequence of status updates!")
