using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace App.Presentation.Frontend
{
	public interface IFrontendState : INotifyPropertyChanged
	{
		ITopLevel TopLevel { get; }
		IReadOnlyList<Job<EngineInstalling>> EngineInstallingJobs { get; }
		event Action<FrontEndError> ErrorOccurred;
	}

	public interface ITopLevel : INotifyPropertyChanged
	{
	}

	public interface IOverview : ITopLevel
	{
		IOverviewVariant ActiveVariant { get; }
		void ToProject();
		void ToEngines();
	}

	public interface IPreferences : ITopLevel
	{
		IConfig Config { get; }
		void ToGeneral();
		void ToUi();
	}

	public interface IOverviewVariant : INotifyPropertyChanged
	{
	}

	public interface IProjects : IOverviewVariant
	{
		IReadOnlyList<Project> Data { get; }
		void CreateProject(string name);
		void DeleteProject(Project project);
	}

	public interface IEngines : IOverviewVariant
	{
		IReadOnlyList<EngineInstall> InstalledEngines { get; }
		IReadOnlyList<Engine> AllEngines { get; }
		EngineInstall? CurrentlyActiveEngine { get; set; }
		void InstallEngine(Engine engine);
		void DeleteEngineInstall(EngineInstall engineInstall);
	}

	public interface IConfig
	{
	}

	public interface IGeneralConfig : IConfig, INotifyPropertyChanged
	{
		ConfigData<DirectoryInfo> ProjectPath { get; set; }
		ConfigData<DirectoryInfo> EnginesInstallationPath { get; set; }
	}

	public interface IUiConfig : IConfig, INotifyPropertyChanged
	{
		ConfigData<Theme> Theme { get; set; }
	}
}
