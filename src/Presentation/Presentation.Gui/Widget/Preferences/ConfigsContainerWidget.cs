using System;
using App.Presentation.Frontend;
using Gtk;

namespace App.Presentation.Gui
{
	public class ConfigsContainerWidget : Box
	{
		private const int LabelStartMargin = 25;

		public ConfigsContainerWidget(IConfigContainerFrontend viewModel)
			: base(Orientation.Vertical, Styling.SubLevelSpacing)
		{
			StyleContext.AddClass("border-round");

			var headingLabel = Label.New(viewModel.Heading);
			headingLabel.Xalign = 0;
			headingLabel.MarginStart = LabelStartMargin;
			headingLabel.MarginTop = Styling.SubLevelSpacing;
			headingLabel.MarginBottom = Styling.SubLevelSpacing * 2;
			headingLabel.StyleContext.AddClass("heading-light");
			Add(headingLabel);

			Widget? lastEntryWidget = null;
			foreach (var entry in viewModel.Entries)
			{
				ConfigWidgetBase entryWidget = entry switch
				{
					IConfigDirectoryFullstack dir => new ConfigDirectoryWidget(dir),
					IConfigDropdownFullstack dd => new ConfigDropdownWidget(dd),
					_ => throw new ArgumentOutOfRangeException(nameof(viewModel))
				};

				lastEntryWidget = entryWidget;
				Add(entryWidget);
			}

			if (lastEntryWidget != null)
				lastEntryWidget.MarginBottom = Styling.SubLevelSpacing;
		}
	}
}
