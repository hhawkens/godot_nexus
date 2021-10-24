using System;

namespace App.Presentation.Frontend
{
	/// Implements basic (model-facing) view-model type functionality.
	public abstract class BackendBase<TModel> : IBackend<TModel>
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
			ModelUpdateRequired = null;
		}

		/// <inheritdoc />
		public abstract void NotifyModelUpdated(TModel model);

		/// Inheriting classes implement dispose functionality here.
		protected abstract void BeforeDispose();

		protected void RequestModelUpdate(Action<TModel> modelUpdateAction) =>
			ModelUpdateRequired?.Invoke(modelUpdateAction);
	}
}
