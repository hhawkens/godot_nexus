namespace App.Utilities

open System
open System.IO
open System.Net
open System.Net.Http
open System.Net.NetworkInformation

[<Struct>]
type public DownloadProgress =
    | UnknownProgress
    | DownloadProgress of struct{| Current: FileSize; Total: FileSize |}


[<AutoOpen>]
module private WebInternal =

    let private streamToFile cancel updateProgress (stream: Stream) filename = async {
        use _ = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write)
        use fileStream = new FileStream(filename, FileMode.Truncate, FileAccess.Write)
        let mutable readCumulated = 0UL
        let buffer = Array.zeroCreate 4096

        let rec readBytes () = async {
            let! read = stream.ReadAsync(buffer, 0, buffer.Length, cancel) |> Async.AwaitTask
            return!
                match read with
                | 0 -> async { return () }
                | read -> async {
                    do! fileStream.WriteAsync(buffer, 0, read, cancel) |> Async.AwaitTask
                    do! fileStream.FlushAsync() |> Async.AwaitTask
                    readCumulated <- readCumulated + uint64 read
                    updateProgress (uint64 readCumulated)
                    return! readBytes()
                }
        }
        do! readBytes()
    }

    let private updateProgress (response: HttpResponseMessage) progressHook bytesRead =
        let fileLengthNullable = response.Content.Headers.ContentLength
        let fileLengthOpt = if fileLengthNullable.HasValue then Some fileLengthNullable.Value else None

        match fileLengthOpt with
        | Some len ->
            let progress = DownloadProgress {|Current = {bytes = bytesRead}; Total = {bytes = uint64 len}|}
            progressHook progress
        | None -> progressHook UnknownProgress

    let internal downloadFileUnsafe (uri: Uri) filename cancel progressHook = async {
        use client = new HttpClient()
        use! response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancel) |> Async.AwaitTask

        match response.StatusCode with
        | HttpStatusCode.OK ->
            use! stream = response.Content.ReadAsStreamAsync(cancel) |> Async.AwaitTask
            let progressUpdater = updateProgress response progressHook
            do! streamToFile cancel progressUpdater stream filename
        | err -> failwith $"Unable to download file \"{filename}\", network error: {unbox<int> err} {err}"
        // We are throwing exceptions here because that is what the utilized C# methods do
    }


/// Utility functions to interact with the WWW.
module public Web =

    /// Checks if we can ping given IP address.
    let public pingAsync (ip: string) = async {
        let myPing = new Ping()
        let timeout = 1500
        try
            let! reply = myPing.SendPingAsync(ip, timeout) |> Async.AwaitTask
            return reply.Status = IPStatus.Success
        with | _ -> return false
    }

    /// Checks if we can ping given IP address.
    let public ping ip = pingAsync ip |> Async.RunSynchronously

    /// Checks if we are connected to the internet.
    let checkWebConnectionAsync () = pingAsync "1.1.1.1"

    /// Checks if we are connected to the internet.
    let checkWebConnection () = checkWebConnectionAsync () |> Async.RunSynchronously

    /// Downloads file from a web source. Deletes partially downloaded file if something went wrong.
    let public downloadFileAsync (uri: Uri) (folder: DirectoryData) cancel progressHook = async {
        let filename = uri.LocalPath |> Path.GetFileName
        let filePath = Path.Combine(folder.FullPath, filename)
        let! result =
            exnToResultAsync (fun () -> downloadFileUnsafe uri filePath cancel progressHook)
        if result.IsError && File.Exists(filePath) then do File.Delete(filePath)

        return result |> Result.map (fun _ -> FileData.tryCreate filePath |> unwrap)
    }
