using System.Linq;
using App.Presentation.Frontend;
using Gtk;

namespace App.Presentation.Gui
{
	public class ConfigDropdownWidget : ConfigWidgetBase
	{
		private readonly IConfigDropdownFrontend viewModel;
		private readonly ComboBox dropDown;

		public ConfigDropdownWidget(IConfigDropdownFrontend viewModel)
			: base(viewModel.Name, viewModel.Description)
		{
			this.viewModel = viewModel;

			dropDown = new ComboBox(viewModel.Options.ToArray());
			dropDown.SetSizeRequest(ElementWidth, dropDown.AllocatedHeight);
			UpdateVisibleState();

			Add(dropDown);
			dropDown.Changed += delegate
			{
				UpdateViewModel();
				UpdateVisibleState();
			};
		}

		private void UpdateViewModel()
		{
			viewModel.SetValue(viewModel.Options[dropDown.Active]);
		}

		private void UpdateVisibleState()
		{
			dropDown.Active = viewModel.ActiveIndex;
		}
	}
}
