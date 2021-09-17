using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace App.Presentation.Frontend
{
	/// Base for all frontend config types.
	public interface IConfigFrontend : INotifyPropertyChanged
	{
		string Name { get; }
		string Description { get; }
		string Value { get; }
		string DefaultValue { get; }
		bool IsDefault => Value == DefaultValue;
	}

	/// Frontend config type for dropdown selection.
	public interface IConfigDropdownFrontend : IConfigFrontend
	{
		IReadOnlyList<string> Options { get; }
		int ActiveIndex { get; }
		void SetValue(string newValue);
	}

	/// Frontend config type for directory selection.
	public interface IConfigDirectoryFrontend : IConfigFrontend
	{
		void SetValue(DirectoryInfo newDirectory);
	}
}
