using System;

namespace App.Presentation.Frontend;

/// Disposable with a dispose-event.
public interface IDestructible : IDisposable
{
	/// Called after this object was disposed.
	public event EventHandler? Disposed;
}


/// Simple implementation of the "Destructible" pattern.
public abstract class Destructible : IDestructible
{
	public event EventHandler? Disposed;
	public void Dispose() => Disposed?.Invoke(this, EventArgs.Empty);
}


/// Simple implementation of the "Destructible" pattern.
public abstract record DestructibleRec : IDestructible
{
	public event EventHandler? Disposed;
	public void Dispose() => Disposed?.Invoke(this, EventArgs.Empty);
}
