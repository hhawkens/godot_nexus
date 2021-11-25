using System;
using App.Presentation.Frontend;
using App.Presentation.Utilities;
using Gtk;

namespace App.Presentation.Gui
{
	public class CollectionFrontendBinding<T> where T: notnull
	{
		private readonly ICollectionFrontend<T> frontend;
		private readonly Box parentWidget;
		private readonly Func<T, Widget> entryWidgetCreationMethod;

		internal CollectionFrontendBinding(
			ICollectionFrontend<T> frontend,
			Box parentWidget,
			Func<T, Widget> entryWidgetCreationMethod)
		{
			this.frontend = frontend;
			this.parentWidget = parentWidget;
			this.entryWidgetCreationMethod = entryWidgetCreationMethod;

			this.frontend.Entries.ListChanged += EntriesChangedHandler;

			InitChildren();
		}

		private void EntriesChangedHandler(object? sender, ListChangedEventArgs<T> e)
		{
			switch (e)
			{
				case ListChangedEventArgs<T>.Appended ap: HandleItemAppended(ap.AppendedItem); break;
				case ListChangedEventArgs<T>.Inserted(var item, var idx): HandleItemInserted(item, idx); break;
				case ListChangedEventArgs<T>.Removed(_, var idx): HandleItemRemoved(idx); break;
				case ListChangedEventArgs<T>.Moved(_, var oldIdx, var newIdx): HandleItemMoved(oldIdx, newIdx); break;
				case ListChangedEventArgs<T>.Replaced(_, var @new, var idx): HandleItemReplaced(@new, idx); break;
				case ListChangedEventArgs<T>.Reset: HandleReset(); break;
			}
		}

		private void HandleItemAppended(T item) => parentWidget.Add(CreateWidgetFrom(item));

		private void HandleItemInserted(T item, int index) =>
			parentWidget.InsertAt(index, CreateWidgetFrom(item));

		private void HandleItemRemoved(int index) => parentWidget.Remove(parentWidget.Children[index]);

		private void HandleItemMoved(int oldIndex, int newIndex) =>
			parentWidget.ReorderChild(parentWidget.Children[oldIndex], newIndex);

		private void HandleItemReplaced(T newItem, int index) =>
			parentWidget.Children[index] = CreateWidgetFrom(newItem);

		private void HandleReset()
		{
			parentWidget.RemoveAllChildren();
			InitChildren();
		}

		private void InitChildren()
		{
			foreach (var entry in frontend.Entries)
			{
				parentWidget.Add(CreateWidgetFrom(entry));
			}
		}

		private Widget CreateWidgetFrom(T item) => entryWidgetCreationMethod(item);
	}
}
