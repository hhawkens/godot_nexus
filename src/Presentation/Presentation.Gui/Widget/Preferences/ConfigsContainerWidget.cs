using System.Collections.Generic;
using Gtk;

namespace App.Presentation.Gui
{
	public class ConfigsContainerWidget : Box
	{
		private const int SpacingPixels = 4;
		private const int LabelStartMargin = 25;

		public ConfigsContainerWidget(string heading, IEnumerable<ConfigWidgetBase> entries)
			: base(Orientation.Vertical, SpacingPixels)
		{
			StyleContext.AddClass("border-round");

			var headingLabel = Label.New(heading);
			headingLabel.Xalign = 0;
			headingLabel.MarginStart = LabelStartMargin;
			headingLabel.MarginTop = SpacingPixels;
			headingLabel.MarginBottom = SpacingPixels * 2;
			headingLabel.StyleContext.AddClass("heading-light");
			Add(headingLabel);

			Widget? lastEntry = null;
			foreach (var entry in entries)
			{
				lastEntry = entry;
				Add(entry);
			}

			if (lastEntry != null)
				lastEntry.MarginBottom = SpacingPixels;
		}
	}
}
