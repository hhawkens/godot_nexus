using App.Core.Domain;
using App.Utilities;

namespace App.Presentation.Frontend
{
	public class PreferencesFrontend
	{
		public GeneralConfigContainerFrontend GeneralConfig { get; }
		public UiConfigContainerFrontend UiConfig { get; }

		public PreferencesFrontend(Preferences model)
		{
			var enginesPathConfig = new ConfigDirectoryFrontend(
				nameof(model.General.EnginesPath).SplitPascalCase(), model.General.EnginesPath);
			var projectsPathConfig = new ConfigDirectoryFrontend(
				nameof(model.General.ProjectsPath).SplitPascalCase(), model.General.ProjectsPath);
			GeneralConfig = new GeneralConfigContainerFrontend(
				nameof(model.General), enginesPathConfig, projectsPathConfig);

			var themeConfig = new ConfigDropdownFrontend<Theme>(nameof(model.UI.Theme), model.UI.Theme);
			UiConfig = new UiConfigContainerFrontend(nameof(model.UI), themeConfig);
		}
	}
}
