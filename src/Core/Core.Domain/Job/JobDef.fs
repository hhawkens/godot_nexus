namespace App.Core.Domain

type public IDownloadEngineJob = IJob<EngineZipFile * EngineOnline, ErrorMessage>
type public ICreateNewProjectJob = IJob<Project, ErrorMessage>


/// Contains all possible jobs this app can run.
type public JobDef =
    | DownloadEngine of IDownloadEngineJob
    | CreateProject of ICreateNewProjectJob

with

    member public this.Job =
        match this with
        | DownloadEngine a -> a:>IJob
        | CreateProject a -> a:>IJob
