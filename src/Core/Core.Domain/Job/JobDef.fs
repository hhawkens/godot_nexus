namespace App.Core.Domain

type public IDownloadEngineJob = IJob<EngineZipFile * EngineOnline, ErrorMessage>
type public IInstallEngineJob = IJob<EngineInstall, ErrorMessage>
type public ICreateNewProjectJob = IJob<Project, ErrorMessage>


/// Contains all possible jobs this app can run.
type public JobDef =
    | DownloadEngine of IDownloadEngineJob
    | InstallEngine of IInstallEngineJob
    | CreateProject of ICreateNewProjectJob

with

    member public this.Job =
        match this with
        | DownloadEngine a -> a:>IJob
        | InstallEngine a -> a:>IJob
        | CreateProject a -> a:>IJob
