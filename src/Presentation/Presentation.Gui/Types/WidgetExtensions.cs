using Gtk;

namespace App.Presentation.Gui;

public static class WidgetExtensions
{
	/// Inserts a new child widget at given index.
	/// Returns an error message if something went wrong, e.g. out of bounds index.
	public static string? InsertAt(this Box source, int index, Widget child)
	{
		if (source.Children.Length < index)
		{
			return $"Could not insert widget {child.Name} into parent {source.Name}. " +
			       $"Requested index was {index}, but parent has only {source.Children.Length} elements!";
		}

		if (source.Children.Length == index)
		{
			source.Add(child);
			return null;
		}

		for (var i = index; i <= source.Children.Length; i++)
			source.ReorderChild(source.Children[i], i + 1);
		source.ReorderChild(child, index);

		return null;
	}

	/// Removes and destroys all children from given container.
	public static void RemoveAllChildren(this Container source)
	{
		foreach (var child in source.Children)
			child.SelfDestruct();
	}

	/// Removes this widget from its parent container, then destroys this widget.
	public static void SelfDestruct(this Widget source)
	{
		(source.Parent as Container)?.Remove(source);
		source.Destroy();
	}
}
