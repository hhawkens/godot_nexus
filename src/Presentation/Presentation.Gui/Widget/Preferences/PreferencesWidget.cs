using App.Presentation.Frontend;
using Gtk;

namespace App.Presentation.Gui
{
	public class PreferencesWidget : ScrolledWindow
	{
		private const int Space = 16;

		public PreferencesWidget(IPreferencesFrontend viewModel)
		{
			Margin = Space;

			var generalConfigsContainerWidget = new ConfigsContainerWidget(viewModel.GeneralConfig);
			var uiConfigsContainerWidget = new ConfigsContainerWidget(viewModel.UiConfig);

			var verticalBox = new Box(Orientation.Vertical, Space);
			verticalBox.Add(generalConfigsContainerWidget);
			verticalBox.Add(uiConfigsContainerWidget);

			Add(verticalBox);

			viewModel.Disposed += delegate { this.SelfDestruct(); };
		}
	}
}
