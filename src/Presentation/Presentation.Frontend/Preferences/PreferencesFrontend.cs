using App.Core.Domain;

namespace App.Presentation.Frontend
{
	public class PreferencesFrontend
	{
		public GeneralConfigContainerFrontend GeneralConfig { get; }
		public UiConfigContainerFrontend UiConfig { get; }

		public PreferencesFrontend(Preferences model)
		{
			var enginesPathConfig = new ConfigDirectoryFrontend(model.General.EnginesPath);
			var projectsPathConfig = new ConfigDirectoryFrontend(model.General.ProjectsPath);
			GeneralConfig =
				new GeneralConfigContainerFrontend(nameof(model.General), enginesPathConfig, projectsPathConfig);

			var themeConfig = new ConfigDropdownFrontend<Theme>(model.UI.Theme);
			UiConfig = new UiConfigContainerFrontend(nameof(model.UI), themeConfig);
		}
	}
}
