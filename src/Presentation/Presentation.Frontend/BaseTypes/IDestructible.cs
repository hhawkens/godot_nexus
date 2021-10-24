using System;

namespace App.Presentation.Frontend
{
	/// Disposable with a dispose-event.
	public interface IDestructible : IDisposable
	{
		/// Called after this object was disposed.
		public event EventHandler Disposed;
	}
}
