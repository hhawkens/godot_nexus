using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace App.Presentation.Frontend
{
	/// Base for all frontend config types.
	public interface IConfigFrontend : INotifyPropertyChanged
	{
		string Description { get; }

		string Value { get; }

		string DefaultValue { get; }

		bool IsDefault =>
			Value == DefaultValue;
	}

	/// Frontend config type for dropdown selection.
	public interface IConfigDropdownFrontend
	{
		IReadOnlyList<string> Options { get; }

		void SetValue(string newValue);
	}

	/// Frontend config type for directory selection.
	public interface IConfigDirectoryFrontend
	{
		void SetValue(DirectoryInfo newDirectory);
	}
}
