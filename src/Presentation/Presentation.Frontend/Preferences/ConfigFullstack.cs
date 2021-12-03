using System;
using System.ComponentModel;
using FSharpPlus;

namespace App.Presentation.Frontend
{
	/// <inheritdoc cref="IConfigFrontend" />
	internal abstract record ConfigFrontend<T>(string Name, string Description, T DefaultValue, T Value)
		: DestructibleRec, IConfigFrontend<T>
	{
#pragma warning disable CS0067
		public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

		public T Value { get; protected set; } = Value;
		public abstract bool IsDefault { get; }

		public abstract void SetValue(T newValue);
	}


	/// <inheritdoc cref="IConfigFullstack{TFrontend,TBackend}" />
	internal abstract record ConfigFullstack<TFrontend, TBackend>
		(string Name, string Description, TFrontend DefaultValue, TFrontend Value) :
			ConfigFrontend<TFrontend>(Name, Description, DefaultValue, Value),
			IConfigFullstack<TFrontend, TBackend>,
			IDualConverter<TFrontend, TBackend>
	{
		private int? lastUpdateValueHash;

		public event EventHandler<TBackend>? ModelValueUpdateRequired;

		public void ModelValueUpdatedHandler(TBackend backendValue)
		{
			var newUpdateValueHash = backendValue?.GetHashCode();
			if (lastUpdateValueHash != newUpdateValueHash)
			{
				Value = Convert(backendValue);
				lastUpdateValueHash = newUpdateValueHash;
			}
		}

		public override void SetValue(TFrontend frontendValue) =>
			ModelValueUpdateRequired?.Invoke(this, Convert(frontendValue));

		public abstract TBackend Convert(TFrontend value);
		public abstract TFrontend Convert(TBackend value);
	}
}
