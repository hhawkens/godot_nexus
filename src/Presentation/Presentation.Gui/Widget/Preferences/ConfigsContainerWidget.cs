using System.Collections.Generic;
using Gtk;

namespace App.Presentation.Gui
{
	public class ConfigsContainerWidget : Box
	{
		private const float HeadingXAlign = 0.035f;
		private const int SpacingPixels = 4;

		public ConfigsContainerWidget(string heading, IEnumerable<ConfigWidgetBase> entries)
			: base(Orientation.Vertical, SpacingPixels)
		{
			StyleContext.AddClass("border-round");

			var headingLabel = Label.New(heading);
			headingLabel.Xalign = HeadingXAlign;
			headingLabel.MarginTop = SpacingPixels;
			headingLabel.MarginBottom = SpacingPixels * 2;
			headingLabel.StyleContext.AddClass("heading-light");
			Add(headingLabel);

			Widget? lastEntry = null;
			foreach (var entry in entries)
			{
				Add(entry);
				lastEntry = entry;
			}

			if (lastEntry != null)
				lastEntry.MarginBottom = SpacingPixels;
		}
	}
}
