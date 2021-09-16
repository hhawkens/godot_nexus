using System.Collections.Generic;
using Gtk;

namespace App.Presentation.Gui
{
	public class PreferencesWidget : ScrolledWindow
	{
		private const int Space = 16;

		public PreferencesWidget(IEnumerable<ConfigsContainerWidget> configContainers)
		{
			Margin = Space;

			var verticalBox = new Box(Orientation.Vertical, Space);
			foreach (var configsContainerWidget in configContainers)
				verticalBox.Add(configsContainerWidget);

			Add(verticalBox);
		}
	}
}
