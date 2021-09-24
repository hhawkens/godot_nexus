using System;
using App.Presentation.Frontend;
using Gtk;

namespace App.Presentation.Gui
{
	public class ConfigsContainerWidget : Box
	{
		private const int SpacingPixels = 4;
		private const int LabelStartMargin = 25;

		public ConfigsContainerWidget(IConfigContainerFrontend viewModel)
			: base(Orientation.Vertical, SpacingPixels)
		{
			StyleContext.AddClass("border-round");

			var headingLabel = Label.New(viewModel.Heading);
			headingLabel.Xalign = 0;
			headingLabel.MarginStart = LabelStartMargin;
			headingLabel.MarginTop = SpacingPixels;
			headingLabel.MarginBottom = SpacingPixels * 2;
			headingLabel.StyleContext.AddClass("heading-light");
			Add(headingLabel);

			Widget? lastEntryWidget = null;
			foreach (var entry in viewModel.Entries)
			{
				ConfigWidgetBase entryWidget = entry switch
				{
					IConfigDirectoryFrontend dir => new ConfigDirectoryWidget(dir),
					IConfigDropdownFrontend dd => new ConfigDropdownWidget(dd),
					_ => throw new ArgumentOutOfRangeException(nameof(viewModel))
				};

				lastEntryWidget = entryWidget;
				Add(entryWidget);
			}

			if (lastEntryWidget != null)
				lastEntryWidget.MarginBottom = SpacingPixels;
		}
	}
}
