using System;
using System.Collections.Generic;

namespace App.Presentation.Utilities
{
	/// A read-only list that emits events when the underlying state has changed.
	public interface IObservableReadOnlyList<T> : IReadOnlyList<T>
	{
		/// Called whenever the list has changed, i.e. an item has been added, removed, moved and so on.
		event EventHandler<ListChangedEventArgs<T>>? ListChanged;
	}

	/// A list that emits events when changed.
	// ReSharper disable once PossibleInterfaceMemberAmbiguity
	public interface IObservableList<T> : IObservableReadOnlyList<T>, IList<T>
	{
		/// Move item at oldIndex to newIndex.
		void Move(int oldIndex, int newIndex);
	}
}
