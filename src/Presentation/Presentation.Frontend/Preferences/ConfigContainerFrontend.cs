using System.Collections.Generic;

namespace App.Presentation.Frontend
{
	public interface IConfigContainerFrontend
	{
		string Heading { get; }
		IReadOnlyList<IConfigFrontend> Entries { get; }
	}

	public record GeneralConfigContainerFrontend(
		string Heading, IConfigDirectoryFrontend EnginesPathConfig, IConfigDirectoryFrontend ProjectsPathConfig)
		: IConfigContainerFrontend
	{
		public IReadOnlyList<IConfigFrontend> Entries => new[] { EnginesPathConfig, ProjectsPathConfig };
	}

	public record UiConfigContainerFrontend(
		string Heading, IConfigDropdownFrontend ThemeConfig)
		: IConfigContainerFrontend
	{
		public IReadOnlyList<IConfigFrontend> Entries => new[] { ThemeConfig };
	}
}
