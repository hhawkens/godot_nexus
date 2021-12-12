using App.Presentation.Frontend;
using Gtk;

namespace App.Presentation.Gui;

public class PreferencesWidget : ScrolledWindow
{
	public PreferencesWidget(IPreferencesFrontend viewModel)
	{
		Margin = Styling.TopLevelSpacing;

		var generalConfigsContainerWidget = new ConfigsContainerWidget(viewModel.GeneralConfig);
		var uiConfigsContainerWidget = new ConfigsContainerWidget(viewModel.UiConfig);

		var verticalBox = new Box(Orientation.Vertical, Styling.TopLevelSpacing);
		verticalBox.Add(generalConfigsContainerWidget);
		verticalBox.Add(uiConfigsContainerWidget);

		Add(verticalBox);

		viewModel.Disposed += delegate { this.SelfDestruct(); };
	}
}
