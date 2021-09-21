using System;
using System.Collections.Generic;
using System.Linq;
using App.Core.Domain;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Frontend config type for dropdown selection.
	public interface IConfigDropdownFrontend : IConfigFrontend
	{
		IReadOnlyList<string> Options { get; }
		int ActiveIndex { get; }
		void SetValue(string newValue);
	}


	/// <inheritdoc cref="IConfigDropdownFrontend" />
	public record ConfigDropdownFrontend<T> : ConfigFrontend, IConfigDropdownFrontend where T: Enum
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
}
