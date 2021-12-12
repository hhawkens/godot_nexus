using Gtk;

namespace App.Presentation.Gui;

/// Compound widget that can be used in the sidebar as a list entry
public class SidebarEntry : ListBoxRow
{
	private const string StyleClass = "list-entry";
	private const int Height = 44;

	public SidebarEntry(string text, IconInfo iconInfo)
	{
		HeightRequest = Height;
		StyleContext.AddClass(StyleClass);

		var container = new Box(Orientation.Horizontal, 0)
		{
			iconInfo.CreateGtkImage(),
			new Label(text)
		};

		Add(container);
	}

	public override string ToString() =>
		$"{((Label) ((Container) Children[0]).Children[1]).Text}#{GetHashCode()}";
}
