using System;
using System.Collections.Generic;
using System.Linq;
using App.Core.Domain;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Frontend config type for dropdown selection.
	public interface IConfigDropdownFullstack : IConfigFrontend<string>
	{
		/// The available options of this dropdown.
		IReadOnlyList<string> Options { get; }

		/// Index of the currently selected dropdown option.
		int ActiveIndex { get; }
	}


	/// <inheritdoc cref="IConfigDropdownFullstack" />
	internal record ConfigDropdownFullstack<TBackend> :
		ConfigFullstack<string, TBackend>, IConfigDropdownFullstack
		where TBackend: struct, Enum
	{
		/// <inheritdoc />
		public IReadOnlyList<string> Options => options;

		/// <inheritdoc />
		public int ActiveIndex => Array.IndexOf(options, Value);

		/// <inheritdoc />
		public override bool IsDefault => Value == DefaultValue;

		private readonly string[] options;

		public ConfigDropdownFullstack(string name, ConfigData<TBackend> model)
			: base(name, model.Description, model.DefaultValue.ToString(), model.CurrentValue.ToString())
		{
			options = Enums.iterate<TBackend>().Select(Convert).ToArray();
		}

		/// <inheritdoc />
		public override TBackend Convert(string value) => Enum.Parse<TBackend>(value);

		/// <inheritdoc />
		public override string Convert(TBackend value) => value.ToString();
	}
}
