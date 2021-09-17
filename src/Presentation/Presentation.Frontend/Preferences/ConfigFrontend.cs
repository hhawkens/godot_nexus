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
	public abstract record ConfigFrontend(string Description, string DefaultValue, string Value)
		: IConfigFrontend
	{
		public string Value { get; protected set; } = Value;
#pragma warning disable CS0067
		public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
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
		public ConfigDirectoryFrontend(ConfigData<DirectoryData> model)
			: base(model.Description, model.DefaultValue.ToString(), model.CurrentValue.ToString())
		{
			Value = model.CurrentValue.FullPath;
		}

		public void SetValue(DirectoryInfo newDirectory)
		{
			Value = newDirectory.FullName;
		}
	}
}
