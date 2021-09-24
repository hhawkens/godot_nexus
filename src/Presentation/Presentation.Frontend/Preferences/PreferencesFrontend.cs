using App.Core.Domain;
using App.Shell.State;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// View model abstraction for the UI of the preferences panel.
	public class PreferencesFrontend : FrontendBase
	{
		public GeneralConfigContainerFrontend GeneralConfig { get; }
		public UiConfigContainerFrontend UiConfig { get; }

		private readonly IPreferencesStateController model;
		private readonly ConfigDirectoryFrontend enginesPathConfig;
		private readonly ConfigDirectoryFrontend projectsPathConfig;
		private readonly ConfigDropdownFrontend<Theme> themeConfig;

		public PreferencesFrontend(IPreferencesStateController model)
		{
			this.model = model;
			var prefs = this.model.Preferences;

			enginesPathConfig = CreateEnginesPathConfig(prefs.General.EnginesPath);
			projectsPathConfig = CreateProjectsPathConfig(prefs.General.ProjectsPath);
			GeneralConfig = new GeneralConfigContainerFrontend(nameof(prefs.General),
				enginesPathConfig,
				projectsPathConfig);

			themeConfig = CreateThemeConfig(prefs.UI.Theme);
			UiConfig = new UiConfigContainerFrontend(nameof(prefs.UI),
				themeConfig);

			this.model.PreferencesChanged.AddHandler(ModelChangedHandler);
			enginesPathConfig.FrontendChanged += EnginesPathChanged;
			projectsPathConfig.FrontendChanged += ProjectsPathChanged;
			themeConfig.FrontendChanged += ThemeConfigChanged;
		}

		/// <inheritdoc />
		protected override void OnDispose() => model.PreferencesChanged.RemoveHandler(ModelChangedHandler);

		private void ModelChangedHandler(object sender, Preferences prefs)
		{
			enginesPathConfig.ModelUpdatedHandler(prefs.General.EnginesPath.CurrentValue.FullPath);
			projectsPathConfig.ModelUpdatedHandler(prefs.General.ProjectsPath.CurrentValue.FullPath);
			themeConfig.ModelUpdatedHandler(prefs.UI.Theme.CurrentValue);
		}

		private void EnginesPathChanged(object? sender, string e) => model.SetEnginesPathConfig(e);
		private void ProjectsPathChanged(object? sender, string e) => model.SetProjectsPathConfig(e);
		private void ThemeConfigChanged(object? sender, Theme e) => model.SetThemeConfig(e);

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
