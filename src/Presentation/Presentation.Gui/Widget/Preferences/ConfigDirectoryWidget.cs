using System.IO;
using App.Presentation.Frontend;
using GLib;
using Gtk;

namespace App.Presentation.Gui
{
	public class ConfigDirectoryWidget : ConfigWidgetBase
	{
		private readonly IConfigDirectoryFrontend viewModel;
		private readonly FileChooserButton chooser;

		public ConfigDirectoryWidget(IConfigDirectoryFrontend viewModel)
			: base(viewModel.Name, viewModel.Description)
		{
			this.viewModel = viewModel;

			chooser = new FileChooserButton("Choose Directory", FileChooserAction.SelectFolder);
			chooser.SetSizeRequest(ElementWidth, chooser.AllocatedHeight);
			UpdateVisibleState();

			Add(chooser);
			chooser.FileSet += delegate
			{
				UpdateViewModel();
				UpdateVisibleState();
			};
		}

		private void UpdateViewModel()
		{
			viewModel.SetValue(new DirectoryInfo(chooser.File.Uri.AbsolutePath));
		}

		private void UpdateVisibleState()
		{
			chooser.SetFile(FileFactory.NewForPath(viewModel.Value));
			chooser.TooltipText = chooser.File.Uri.AbsolutePath;
		}
	}
}
