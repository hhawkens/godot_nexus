using System;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Implements basic frontend type functionality.
	public abstract class FrontendBase : IDestructible
	{
		/// <inheritdoc />
		public event DestructibleEventHandler? Disposed;

		/// <inheritdoc />
		public void Dispose()
		{
			OnDispose();
			Disposed?.Invoke(this, EventArgs.Empty);
			Disposed = null;
		}

		/// Inheriting classes implement dispose functionality here.
		protected abstract void OnDispose();
	}
}
