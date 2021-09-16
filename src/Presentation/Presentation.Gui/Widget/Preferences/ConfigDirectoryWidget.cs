using GLib;
using Gtk;

namespace App.Presentation.Gui
{
	public class ConfigDirectoryWidget : ConfigWidgetBase
	{
		private readonly FileChooserButton chooser;

		public ConfigDirectoryWidget(string label, string labelTooltip) : base(label, labelTooltip)
		{
			chooser = new FileChooserButton("Choose Directory", FileChooserAction.SelectFolder);
			chooser.SetSizeRequest(ElementWidth, chooser.AllocatedHeight);
			chooser.FileSet += delegate { UpdateTooltip(); };
			chooser.SetFile(FileFactory.NewForPath("/home"));
			UpdateTooltip();

			Add(chooser);
		}

		private void UpdateTooltip()
		{
			chooser.TooltipText = chooser.File.Uri.AbsolutePath;
		}
	}
}
