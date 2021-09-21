using System.Collections.Generic;

namespace App.Presentation.Frontend
{
	/// Collection of connected configurations.
	public interface IConfigContainerFrontend
	{
		string Heading { get; }
		IReadOnlyList<IConfigFrontend> Entries { get; }
	}


	/// <inheritdoc cref="IConfigContainerFrontend"/>
	public record GeneralConfigContainerFrontend(
		string Heading, IConfigDirectoryFrontend EnginesPathConfig, IConfigDirectoryFrontend ProjectsPathConfig)
		: IConfigContainerFrontend
	{
		public IReadOnlyList<IConfigFrontend> Entries => new[] { EnginesPathConfig, ProjectsPathConfig };
	}


	/// <inheritdoc cref="IConfigContainerFrontend"/>
	public record UiConfigContainerFrontend(
		string Heading, IConfigDropdownFrontend ThemeConfig)
		: IConfigContainerFrontend
	{
		public IReadOnlyList<IConfigFrontend> Entries => new[] { ThemeConfig };
	}
}
