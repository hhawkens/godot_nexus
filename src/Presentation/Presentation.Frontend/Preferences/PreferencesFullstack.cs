using System;
using App.Core.Domain;
using App.Shell.State;
using App.Utilities;
using Microsoft.FSharp.Core;

namespace App.Presentation.Frontend
{
	/// View model abstraction for the UI of the preferences panel.
	public class PreferencesFullstack : FrontendBase<IPreferencesStateController>, IPreferencesFullstack
	{
		public GeneralConfigContainerFrontend GeneralConfig { get; }
		public UiConfigContainerFrontend UiConfig { get; }

		private readonly Action<Error> errorThrower;
		private readonly ConfigDirectoryFrontend enginesPathFrontend;
		private readonly ConfigDirectoryFrontend projectsPathFrontend;
		private readonly ConfigDropdownFrontend<Theme> themeFrontend;

		public PreferencesFullstack(
			Preferences initialPreferences,
			Action<Error> errorThrower)
		{
			this.errorThrower = errorThrower;

			var prefs = initialPreferences;
			enginesPathFrontend = CreateEnginesPathConfig(prefs.General.EnginesPath);
			projectsPathFrontend = CreateProjectsPathConfig(prefs.General.ProjectsPath);
			GeneralConfig = new GeneralConfigContainerFrontend(nameof(prefs.General),
				enginesPathFrontend,
				projectsPathFrontend);

			themeFrontend = CreateThemeConfig(prefs.UI.Theme);
			UiConfig = new UiConfigContainerFrontend(nameof(prefs.UI),
				themeFrontend);

			enginesPathFrontend.FrontendChanged += EnginesPathFrontendChanged;
			projectsPathFrontend.FrontendChanged += ProjectsPathFrontendChanged;
			themeFrontend.FrontendChanged += ThemeFrontendChanged;
		}

		/// <inheritdoc />
		protected override void BeforeDispose() { }

		/// <inheritdoc />
		public void NotifyModelChanged(Preferences prefs)
		{
			enginesPathFrontend.ModelUpdatedHandler(prefs.General.EnginesPath.CurrentValue.FullPath);
			projectsPathFrontend.ModelUpdatedHandler(prefs.General.ProjectsPath.CurrentValue.FullPath);
			themeFrontend.ModelUpdatedHandler(prefs.UI.Theme.CurrentValue);
		}

		private void EnginesPathFrontendChanged(object? sender, string e) =>
			TryRequestModelUpdate(x => x.SetEnginesPathConfig(e));

		private void ProjectsPathFrontendChanged(object? sender, string e) =>
			TryRequestModelUpdate(x => x.SetProjectsPathConfig(e));

		private void ThemeFrontendChanged(object? sender, Theme e) =>
			TryRequestModelUpdate(x => x.SetThemeConfig(e));

		private void TryRequestModelUpdate<T>(Func<IPreferencesStateController, FSharpResult<T, string>> update) =>
			RequestModelUpdate(x => ThrowIfError(update(x)));

		private void ThrowIfError<T>(FSharpResult<T, string> result)
		{
			if (result.IsError)
				errorThrower(Error.General(result.ErrorValue));
		}

		private static ConfigDirectoryFrontend CreateEnginesPathConfig(ConfigData<DirectoryData> model)
		{
			const string configName = nameof(Preferences.General.EnginesPath);
			return new ConfigDirectoryFrontend(configName.SplitPascalCase(), model);
		}

		private static ConfigDirectoryFrontend CreateProjectsPathConfig(ConfigData<DirectoryData> model)
		{
			const string configName = nameof(Preferences.General.ProjectsPath);
			return new ConfigDirectoryFrontend(configName.SplitPascalCase(), model);
		}

		private static ConfigDropdownFrontend<Theme> CreateThemeConfig(ConfigData<Theme> model)
		{
			const string configName = nameof(Preferences.UI.Theme);
			return new ConfigDropdownFrontend<Theme>(configName.SplitPascalCase(), model);
		}
	}
}
