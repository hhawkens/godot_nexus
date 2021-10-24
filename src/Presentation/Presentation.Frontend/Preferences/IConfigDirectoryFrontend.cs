using System.IO;
using App.Core.Domain;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Frontend config type for directory selection.
	public interface IConfigDirectoryFrontend : IConfigFrontend<DirectoryInfo>
	{
	}


	/// <inheritdoc cref="IConfigDirectoryFrontend" />
	internal record ConfigDirectoryFrontend : ConfigFullstack<DirectoryInfo, string>, IConfigDirectoryFrontend
	{
		/// <inheritdoc />
		public override bool IsDefault => Value.FullName == DefaultValue.FullName;

		public ConfigDirectoryFrontend(string name, ConfigData<DirectoryData> model)
			: base(name, model.Description, ToDirInfo(model), ToDirInfo(model))
		{
			Value = ToDirInfo(model);
		}

		/// <inheritdoc />
		public override string Convert(DirectoryInfo value) => value.FullName;

		/// <inheritdoc />
		public override DirectoryInfo Convert(string value) => new(value);

		private static DirectoryInfo ToDirInfo(ConfigData<DirectoryData> model) => new(model.CurrentValue.FullPath);
	}
}
