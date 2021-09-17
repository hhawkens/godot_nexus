using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using App.Core.Domain;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// <inheritdoc cref="IConfigFrontend" />
	public abstract record ConfigFrontend(string Label, string DefaultValue, string Value)
		: IConfigFrontend
	{
		public string Value { get; protected set; } = Value;
		public event PropertyChangedEventHandler? PropertyChanged;
	}

	/// <inheritdoc cref="IConfigDropdownFrontend" />
	public record ConfigDropdownFrontend<T>
		: ConfigFrontend, IConfigDropdownFrontend
		where T: Enum
	{
		public IReadOnlyList<string> Options { get; }

		public ConfigDropdownFrontend(ConfigData<T> model)
			: base(model.Description, model.DefaultValue.ToString(), model.CurrentValue.ToString())
		{
			Options = Enums.iterate<T>().Select(x => x.ToString()).ToList();
		}

		public void SetValue(string newValue)
		{
			Value = newValue;
		}
	}

	/// <inheritdoc cref="IConfigDirectoryFrontend" />
	public record ConfigDirectoryFrontend
		: ConfigFrontend, IConfigDirectoryFrontend
	{
		public DirectoryInfo Directory { get; private set; }

		public ConfigDirectoryFrontend(ConfigData<DirectoryData> model)
			: base(model.Description, model.DefaultValue.ToString(), model.CurrentValue.ToString())
		{
			Directory = new DirectoryInfo(model.CurrentValue.FullPath);
			Value = model.CurrentValue.FullPath;
		}

		public void SetValue(DirectoryInfo newDirectory)
		{
			Directory = newDirectory;
			Value = newDirectory.FullName;
		}
	}
}
