using System;
using App.Core.Domain;
using App.Shell.State;
using App.Utilities;
using Microsoft.FSharp.Core;

namespace App.Presentation.Frontend
{
	/// View model abstraction for the UI of the preferences panel.
	public class PreferencesFullstack : BackendBase<IPreferencesStateController>, IPreferencesFullstack
	{
		public GeneralConfigContainerFrontend GeneralConfig { get; }
		public UiConfigContainerFrontend UiConfig { get; }

		private readonly Action<Error> errorThrower;
		private readonly ConfigDirectoryFullstack enginesPathFullstack;
		private readonly ConfigDirectoryFullstack projectsPathFullstack;
		private readonly ConfigDropdownFullstack<Theme> themeFullstack;

		public PreferencesFullstack(
			Preferences initialPreferences,
			Action<Error> errorThrower)
		{
			this.errorThrower = errorThrower;

			var prefs = initialPreferences;
			enginesPathFullstack = CreateEnginesPathConfig(prefs.General.EnginesPath);
			projectsPathFullstack = CreateProjectsPathConfig(prefs.General.ProjectsPath);
			GeneralConfig = new GeneralConfigContainerFrontend(nameof(prefs.General),
				enginesPathFullstack,
				projectsPathFullstack);

			themeFullstack = CreateThemeConfig(prefs.UI.Theme);
			UiConfig = new UiConfigContainerFrontend(nameof(prefs.UI),
				themeFullstack);

			enginesPathFullstack.ModelValueUpdateRequired += EnginesPathFullstackChanged;
			projectsPathFullstack.ModelValueUpdateRequired += ProjectsPathFullstackChanged;
			themeFullstack.ModelValueUpdateRequired += ThemeFullstackChanged;
		}

		/// <inheritdoc />
		protected override void BeforeDispose() { }

		/// <inheritdoc />
		public override void NotifyModelUpdated(IPreferencesStateController model)
		{
			var general = model.Preferences.General;
			var ui = model.Preferences.UI;
			enginesPathFullstack.ModelValueUpdatedHandler(general.EnginesPath.CurrentValue.FullPath);
			projectsPathFullstack.ModelValueUpdatedHandler(general.ProjectsPath.CurrentValue.FullPath);
			themeFullstack.ModelValueUpdatedHandler(ui.Theme.CurrentValue);
		}

		private void EnginesPathFullstackChanged(object? sender, string e) =>
			TryRequestModelUpdate(x => x.SetEnginesPathConfig(e));

		private void ProjectsPathFullstackChanged(object? sender, string e) =>
			TryRequestModelUpdate(x => x.SetProjectsPathConfig(e));

		private void ThemeFullstackChanged(object? sender, Theme e) =>
			TryRequestModelUpdate(x => x.SetThemeConfig(e));

		private void TryRequestModelUpdate<T>(Func<IPreferencesStateController, FSharpResult<T, string>> update) =>
			RequestModelUpdate(x => ThrowIfError(update(x)));

		private void ThrowIfError<T>(FSharpResult<T, string> result)
		{
			if (result.IsError)
				errorThrower(Error.General(result.ErrorValue));
		}

		private static ConfigDirectoryFullstack CreateEnginesPathConfig(ConfigData<DirectoryData> model)
		{
			const string configName = nameof(Preferences.General.EnginesPath);
			return new ConfigDirectoryFullstack(configName.SplitPascalCase(), model);
		}

		private static ConfigDirectoryFullstack CreateProjectsPathConfig(ConfigData<DirectoryData> model)
		{
			const string configName = nameof(Preferences.General.ProjectsPath);
			return new ConfigDirectoryFullstack(configName.SplitPascalCase(), model);
		}

		private static ConfigDropdownFullstack<Theme> CreateThemeConfig(ConfigData<Theme> model)
		{
			const string configName = nameof(Preferences.UI.Theme);
			return new ConfigDropdownFullstack<Theme>(configName.SplitPascalCase(), model);
		}
	}
}
