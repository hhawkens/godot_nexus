using System.IO;
using App.Core.Domain;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Frontend config type for directory selection.
	public interface IConfigDirectoryFrontend : IConfigFrontend
	{
		void SetValue(DirectoryInfo newDirectory);
	}


	/// <inheritdoc cref="IConfigDirectoryFrontend" />
	public record ConfigDirectoryFrontend : ConfigFrontend, IConfigDirectoryFrontend
	{
		public ConfigDirectoryFrontend(string name, ConfigData<DirectoryData> model)
			: base(name, model.Description, model.DefaultValue.FullPath, model.CurrentValue.FullPath)
		{
			Value = model.CurrentValue.FullPath;
		}

		public void SetValue(DirectoryInfo newDirectory)
		{
			Value = newDirectory.FullName;
		}
	}
}
