using System;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Basic interface for frontend types.
	public interface IFrontend<in TModel> : IDestructible
	{
		/// Signals that the frontend wants to update its model.
		/// The actual update is performed by whoever listens to this event.
		event Action<Action<TModel>> ModelUpdateRequired;
	}
}
