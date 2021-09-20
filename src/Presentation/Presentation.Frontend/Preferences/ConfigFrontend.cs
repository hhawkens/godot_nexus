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
	public abstract record ConfigFrontend(string Name, string Description, string DefaultValue, string Value)
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
		public IReadOnlyList<string> Options => options;
		public int ActiveIndex => Array.IndexOf(options, Value);

		private readonly string[] options;

		public ConfigDropdownFrontend(string name, ConfigData<T> model)
			: base(name, model.Description, model.DefaultValue.ToString(), model.CurrentValue.ToString())
		{
			options = Enums.iterate<T>().Select(x => x.ToString()).ToArray();
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
