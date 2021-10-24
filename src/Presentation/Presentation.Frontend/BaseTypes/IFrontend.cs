using System;

namespace App.Presentation.Frontend
{
	/// Basic interface for frontend types.
	public interface IFrontend<in TModel> : IDestructible
	{
		/// Signals that the frontend wants to update its model.
		/// The actual update is performed by whoever listens to this event.
		event Action<Action<TModel>> ModelUpdateRequired;

		/// Lets the frontend know that the model data has changed, and which data.
		public void NotifyModelUpdated(TModel model);
	}
}