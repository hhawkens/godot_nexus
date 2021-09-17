namespace App.Presentation.Frontend
{
	public record GeneralConfigContainerFrontend(
		string Heading, IConfigDirectoryFrontend EnginesPathConfig, IConfigDirectoryFrontend ProjectsPathConfig);

	public record UiConfigContainerFrontend(
		string Heading, IConfigDropdownFrontend ThemeConfig);
}
