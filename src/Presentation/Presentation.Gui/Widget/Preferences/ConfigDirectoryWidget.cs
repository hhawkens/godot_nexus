using System.Diagnostics;
using System.IO;
using App.Presentation.Frontend;
using GLib;
using Gtk;
using Process = System.Diagnostics.Process;

namespace App.Presentation.Gui;

/// Config widget that lets the user choose a value from a directory picker.
public class ConfigDirectoryWidget : ConfigWidgetBase
{
	private readonly IConfigDirectoryFullstack viewModel;
	private readonly FileChooserButton chooser;

	public ConfigDirectoryWidget(IConfigDirectoryFullstack viewModel) : base(viewModel)
	{
		this.viewModel = viewModel;

		var valueBox = new Box(Orientation.Horizontal, 0);
		valueBox.Add(chooser = CreateFileChooser());
		valueBox.Add(CreateShowInFilesButton());
		valueBox.SetSizeRequest(ElementWidth, chooser.AllocatedHeight);
		Add(valueBox);

		UpdateVisibleState();
		chooser.FileSet += delegate { UpdateViewModel(); };
		viewModel.PropertyChanged += delegate { UpdateVisibleState(); };
	}

	protected override void ResetToDefault() => viewModel.SetValue(viewModel.DefaultValue);

	private static FileChooserButton CreateFileChooser() => new("Choose Path", FileChooserAction.SelectFolder);

	private Widget CreateShowInFilesButton()
	{
		var showInFilesButtonContent = new ButtonContent(
			new IconInfo(IconType.Goto, ThemeTones.PresetThemeTone),
			delegate { ShowCurrentDirectoryInFiles(); });
		var showInFilesButton = showInFilesButtonContent.ToGtkButton();
		showInFilesButton.TooltipText = "Show in Files";
		return showInFilesButton;
	}

	private void UpdateVisibleState()
	{
		chooser.SetFile(FileFactory.NewForPath(viewModel.Value.FullName));
		chooser.TooltipText = chooser.File.Uri.AbsolutePath;
	}

	private void ShowCurrentDirectoryInFiles()
	{
		var startInfo = new ProcessStartInfo(viewModel.Value.FullName) { UseShellExecute = true };
		Process.Start(startInfo);
	}

	private void UpdateViewModel() => viewModel.SetValue(new DirectoryInfo(chooser.File.Uri.AbsolutePath));
}
