using System.ComponentModel;

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


	/// <inheritdoc cref="IConfigFrontend" />
	public abstract record ConfigFrontend(string Name, string Description, string DefaultValue, string Value)
		: IConfigFrontend
	{
		public string Value { get; protected set; } = Value;

#pragma warning disable CS0067
		public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
	}
}
