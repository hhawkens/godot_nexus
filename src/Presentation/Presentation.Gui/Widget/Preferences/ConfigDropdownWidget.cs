using Gtk;

namespace App.Presentation.Gui
{
	public class ConfigDropdownWidget : ConfigWidgetBase
	{
		public ConfigDropdownWidget(string text, string labelTooltip) : base(text, labelTooltip)
		{
			string[] distros = {
				"Ubuntu",
				"Mandriva",
				"Red Hat",
				"Fedora",
				"Gentoo"
			};

			var dropDown = new ComboBox(distros);
			dropDown.Active = 0;
			dropDown.SetSizeRequest(ElementWidth, dropDown.AllocatedHeight);

			Add(dropDown);
		}
	}
}
