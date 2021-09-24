using System;
using System.ComponentModel;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// <inheritdoc cref="IConfigFrontend" />
	internal abstract record ConfigFrontend<T>(string Name, string Description, T DefaultValue, T Value)
		: IConfigFrontend<T>
	{
#pragma warning disable CS0067
		public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

		public T Value { get; protected set; } = Value;
		public abstract bool IsDefault { get; }

		public abstract void SetValue(T newValue);
	}

	/// <inheritdoc cref="IConfigFullStack{TFrontendValue, TBackendValue}" />
	internal abstract record ConfigFullStack<TFrontend, TBackend>
		(string Name, string Description, TFrontend DefaultValue, TFrontend Value) :
			ConfigFrontend<TFrontend>(Name, Description, DefaultValue, Value),
			IConfigFullStack<TFrontend, TBackend>,
			IDualConverter<TFrontend, TBackend>
	{
		private int? lastUpdateValueHash;

		public event EventHandler<TBackend>? FrontendChanged;

		public void ModelUpdatedHandler(TBackend backendValue)
		{
			var newUpdateValueHash = backendValue?.GetHashCode();
			if (lastUpdateValueHash != newUpdateValueHash)
			{
				Value = Convert(backendValue);
				lastUpdateValueHash = newUpdateValueHash;
			}
		}

		public override void SetValue(TFrontend frontendValue) =>
			FrontendChanged?.Invoke(this, Convert(frontendValue));

		public abstract TBackend Convert(TFrontend value);
		public abstract TFrontend Convert(TBackend value);
	}
}
