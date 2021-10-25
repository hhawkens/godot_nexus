using System.IO;
using App.Core.Domain;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Frontend config type for directory selection.
	public interface IConfigDirectoryFullstack : IConfigFrontend<DirectoryInfo>
	{
	}


	/// <inheritdoc cref="IConfigDirectoryFullstack" />
	internal record ConfigDirectoryFullstack : ConfigFullstack<DirectoryInfo, string>, IConfigDirectoryFullstack
	{
		/// <inheritdoc />
		public override bool IsDefault => Value.FullName == DefaultValue.FullName;

		public ConfigDirectoryFullstack(string name, ConfigData<DirectoryData> model)
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
