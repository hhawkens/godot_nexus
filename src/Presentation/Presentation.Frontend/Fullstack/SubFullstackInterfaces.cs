using System;

namespace App.Presentation.Frontend
{
	/// Describes the GUI facing interface of a view-model/fullstack.
	/// This variant is does not interact with a mutable model, but with
	/// onw of the model's values. Typically this object is managed by another
	/// (non-Sub) Fullstack.
	public interface ISubFrontend : IDestructible
	{
	}


	/// Describes the model facing interface of a view-model/fullstack.
	/// This variant is does not interact with a mutable model, but with
	/// onw of the model's values. Typically this object is managed by another
	/// (non-Sub) Fullstack.
	public interface ISubBackend<T> : IDestructible
	{
		/// Invoked when the frontend wants to change a value of a mutable model.
		/// T is the value.
		event EventHandler<T>? ModelValueUpdateRequired;

		/// Called when the frontend needs to be updated by a value of a model.
		/// T is the value.
		void ModelValueUpdatedHandler(T backendValue);
	}
}
