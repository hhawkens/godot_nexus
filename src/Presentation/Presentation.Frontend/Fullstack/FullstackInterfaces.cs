using System;
using App.Presentation.Utilities;

namespace App.Presentation.Frontend
{
	/// Describes the GUI facing interface of a view-model/fullstack.
	public interface IFrontend : IDestructible
	{
	}


	/// <inheritdoc cref="IFrontend"/>
	/// Additionally, has a collection of child entries.
	public interface ICollectionFrontend<T> : IFrontend
	{
		/// Managed child entries.
		IObservableReadOnlyList<T> Entries { get; }
	}


	/// Describes the model facing interface of a view-model/fullstack.
	public interface IBackend<in TModel> : IDestructible
	{
		/// Signals that this view model wants to update its model.
		/// The actual update is performed by whoever listens to this event.
		event Action<Action<TModel>> ModelUpdateRequired;

		/// Lets this view model know that the model data has changed, and which data.
		public void NotifyModelUpdated(TModel model);
	}
}
