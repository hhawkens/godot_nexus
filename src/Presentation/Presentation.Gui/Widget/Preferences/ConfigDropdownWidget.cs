using System.Linq;
using App.Presentation.Frontend;
using Gtk;

namespace App.Presentation.Gui;

/// Config widget that lets the user choose a value from a dropdown menu.
public class ConfigDropdownWidget : ConfigWidgetBase
{
	private readonly IConfigDropdownFullstack viewModel;
	private readonly ComboBox dropDown;

	public ConfigDropdownWidget(IConfigDropdownFullstack viewModel) : base(viewModel)
	{
		this.viewModel = viewModel;

		dropDown = new ComboBox(this.viewModel.Options.ToArray());
		dropDown.SetSizeRequest(ElementWidth, dropDown.AllocatedHeight);
		UpdateVisibleState();

		Add(dropDown);

		dropDown.Changed += delegate { UpdateViewModel(); };
		this.viewModel.PropertyChanged += delegate { UpdateVisibleState(); };
	}

	protected override void ResetToDefault() => viewModel.SetValue(viewModel.DefaultValue);

	private void UpdateViewModel() => viewModel.SetValue(viewModel.Options[dropDown.Active]);

	private void UpdateVisibleState() => dropDown.Active = viewModel.ActiveIndex;
}
