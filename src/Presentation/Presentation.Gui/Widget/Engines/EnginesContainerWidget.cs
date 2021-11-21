using Gtk;

namespace App.Presentation.Gui
{
	public class EnginesContainerWidget : ScrolledWindow
	{
		private const int Space = 16;

		public EnginesContainerWidget()
		{
			MarginTop = Space;
			MarginBottom = Space;
			var verticalBox = new Box(Orientation.Vertical, Space);
			Add(verticalBox);
		}
	}
}
