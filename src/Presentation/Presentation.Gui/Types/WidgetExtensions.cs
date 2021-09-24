using Gtk;

namespace App.Presentation.Gui
{
	public static class WidgetExtensions
	{
		/// Removes this widget from its parent container, then destroys this widget.
		public static void SelfDestruct(this Widget source)
		{
			(source.Parent as Container)?.Remove(source);
			source.Destroy();
		}
	}
}
