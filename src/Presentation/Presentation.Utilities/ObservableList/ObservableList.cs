using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace App.Presentation.Utilities
{
	/// <inheritdoc />
	public class ObservableList<T> : IObservableList<T>
	{
		private readonly ObservableCollection<T> backendCollection;

		public ObservableList() : this(Array.Empty<T>())
		{
		}

		public ObservableList(IEnumerable<T> enumerable)
		{
			backendCollection = new ObservableCollection<T>(enumerable);
			backendCollection.CollectionChanged += BackendChanged;
		}

		/// <inheritdoc />
		public event EventHandler<ListChangedEventArgs<T>>? ListChanged;

		/// <inheritdoc />
		public void Move(int oldIndex, int newIndex) => backendCollection.Move(oldIndex, newIndex);

		private void BackendChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add: HandleItemAdded(e); break;
				case NotifyCollectionChangedAction.Remove: HandleItemRemoved(e); break;
				case NotifyCollectionChangedAction.Replace: HandleItemReplaced(e); break;
				case NotifyCollectionChangedAction.Move: HandleItemMoved(e); break;
				case NotifyCollectionChangedAction.Reset: HandleCollectionReset(); break;
			}
		}

		private void HandleItemAdded(NotifyCollectionChangedEventArgs e)
		{
			var newItem = (T) e.NewItems![0]!;

			if (e.NewStartingIndex == backendCollection.Count - 1)
				InvokeListChanged(ListChangedEventArgs<T>.Append(newItem, e.NewStartingIndex));
			else
				InvokeListChanged(ListChangedEventArgs<T>.Insert(newItem, e.NewStartingIndex));
		}

		private void HandleItemRemoved(NotifyCollectionChangedEventArgs e)
		{
			var oldItem = (T) e.OldItems![0]!;
			InvokeListChanged(ListChangedEventArgs<T>.Remove(oldItem, e.OldStartingIndex));
		}

		private void HandleItemMoved(NotifyCollectionChangedEventArgs e)
		{
			var movedItem = (T) e.NewItems![0]!;
			InvokeListChanged(ListChangedEventArgs<T>.Move(movedItem, e.OldStartingIndex, e.NewStartingIndex));
		}

		private void HandleItemReplaced(NotifyCollectionChangedEventArgs e)
		{
			var oldItem = (T) e.OldItems![0]!;
			var newItem = (T) e.NewItems![0]!;
			InvokeListChanged(ListChangedEventArgs<T>.Replace(oldItem, newItem, e.NewStartingIndex));
		}

		private void HandleCollectionReset() => InvokeListChanged(ListChangedEventArgs<T>.DoReset());

		private void InvokeListChanged(ListChangedEventArgs<T> args) => ListChanged?.Invoke(this, args);


		// I(Readonly)List implementation forwarding ----------------------------------------------------
		public int Count => backendCollection.Count;
		public bool IsReadOnly => false;
		public IEnumerator<T> GetEnumerator() => backendCollection.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => backendCollection.GetEnumerator();
		public void Add(T item) => backendCollection.Add(item);
		public void Insert(int index, T item) => backendCollection.Insert(index, item);
		public void CopyTo(T[] array, int arrayIndex) => backendCollection.CopyTo(array, arrayIndex);
		public bool Remove(T item) => backendCollection.Remove(item);
		public void RemoveAt(int index) => backendCollection.RemoveAt(index);
		public void Clear() => backendCollection.Clear();
		public int IndexOf(T item) => backendCollection.IndexOf(item);
		public bool Contains(T item) => backendCollection.Contains(item);
		public T this[int index]
		{
			get => backendCollection[index];
			set => backendCollection[index] = value;
		}
	}
}
