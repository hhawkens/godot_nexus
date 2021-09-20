using System.IO;
using App.Presentation.Frontend;
using GLib;
using Gtk;

namespace App.Presentation.Gui
{
	/// Config widget that lets the user choose a value from a directory picker.
	public class ConfigDirectoryWidget : ConfigWidgetBase
	{
		private readonly IConfigDirectoryFrontend viewModel;
		private readonly FileChooserButton chooser;

		public ConfigDirectoryWidget(IConfigDirectoryFrontend viewModel) : base(viewModel)
		{
			this.viewModel = viewModel;

			chooser = new FileChooserButton("Choose Directory", FileChooserAction.SelectFolder);
			chooser.SetSizeRequest(ElementWidth, chooser.AllocatedHeight);
			UpdateVisibleState();

			Add(chooser);

			chooser.FileSet += delegate { UpdateViewModel(); };
			viewModel.PropertyChanged += delegate { UpdateVisibleState(); };
		}

		protected override void ResetToDefault() => viewModel.SetValue(new DirectoryInfo(viewModel.DefaultValue));

		private void UpdateViewModel() => viewModel.SetValue(new DirectoryInfo(chooser.File.Uri.AbsolutePath));

		private void UpdateVisibleState()
		{
			chooser.SetFile(FileFactory.NewForPath(viewModel.Value));
			chooser.TooltipText = chooser.File.Uri.AbsolutePath;
		}
	}
}
