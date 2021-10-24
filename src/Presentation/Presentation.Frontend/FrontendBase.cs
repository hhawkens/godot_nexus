using System;

namespace App.Presentation.Frontend
{
	/// Implements basic frontend type functionality.
	public abstract class FrontendBase<TModel> : IFrontend<TModel>
	{
		/// <inheritdoc />
		public event EventHandler? Disposed;

		/// <inheritdoc />
		public event Action<Action<TModel>>? ModelUpdateRequired;

		/// <inheritdoc />
		public void Dispose()
		{
			BeforeDispose();
			Disposed?.Invoke(this, EventArgs.Empty);
			Disposed = null;
		}

		/// <inheritdoc />
		public abstract void NotifyModelUpdated(TModel model);

		/// Inheriting classes implement dispose functionality here.
		protected abstract void BeforeDispose();

		protected void RequestModelUpdate(Action<TModel> modelUpdateAction) =>
			ModelUpdateRequired?.Invoke(modelUpdateAction);
	}
}
