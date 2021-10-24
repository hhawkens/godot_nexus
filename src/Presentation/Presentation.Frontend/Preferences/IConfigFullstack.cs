using System;
using System.ComponentModel;

namespace App.Presentation.Frontend
{
	/// Base for all frontend config types.
	public interface IConfigFrontend : INotifyPropertyChanged
	{
		/// Short name of this configuration.
		string Name { get; }

		/// Description of what this configuration does.
		string Description { get; }

		/// Checks whether the current configuration value is is the default value.
		bool IsDefault { get; }
	}


	/// Base for all (gui-facing) frontend config types with value setters.
	public interface IConfigFrontend<T> : IConfigFrontend
	{
		/// The current selected value of the config.
		T Value { get; }

		/// Default value of the config.
		T DefaultValue { get; }

		/// Sets the frontend value, so the change can be propagated to the model.
		void SetValue(T newValue);
	}


	/// Interface for the interaction between frontend and model. Internal only!
	internal interface IConfigBackend<T>
	{
		/// Invoked when the frontend wants to change the model value.
		event EventHandler<T>? FrontendChanged;

		/// Called when the frontend needs to be updated by the model.
		void ModelUpdatedHandler(T backendValue);
	}


	/// Combines gui-facing and model-facing interaction of the config frontend. Internal only!
	internal interface IConfigFullstack<TFrontend, TBackend>
		: IConfigFrontend<TFrontend>, IConfigBackend<TBackend>
	{
	}
}
