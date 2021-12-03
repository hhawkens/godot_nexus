using App.Presentation.Frontend;
using Gtk;
using FSharpPlus;

namespace App.Presentation.Gui
{
	public abstract class ConfigWidgetBase : Box
	{
		protected const int ElementWidth = 270;
		private const int LabelStartMargin = 50;
		private const string ActiveResetTooltip = "Reset to default";
		private const string InactiveResetTooltip = "Cannot reset, is default value already";

		private readonly IConfigFrontend viewModel;
		private readonly Button resetToDefaultsButton;

		protected ConfigWidgetBase(IConfigFrontend viewModel) : base(Orientation.Horizontal, Styling.TopLevelSpacing)
		{
			this.viewModel = viewModel;

			var labelWidget = Label.New(viewModel.Name);
			labelWidget.TooltipText = viewModel.Description;
			labelWidget.MarginStart = LabelStartMargin;
			labelWidget.Xalign = 0;
			labelWidget.SetSizeRequest(ElementWidth, labelWidget.AllocatedHeight);
			base.Add(labelWidget);

			var theme = ThemeTones.PresetThemeTone;
			var resetButtonContent = new ButtonContent(
				new IconInfo(IconType.Reset, theme),
				delegate { ResetToDefault(); });
			resetToDefaultsButton = resetButtonContent.ToGtkButton();
			base.Add(resetToDefaultsButton);
			UpdateResetButton();

			viewModel.PropertyChanged += delegate { UpdateResetButton(); };
		}

		/// We always want new widgets added by inheritors to be placed BEFORE the last widget,
		/// which we are adding in this class.
		protected new void Add(Widget widget)
		{
			base.Add(widget);
			Operator.assertThat($"Expected >=2 children in widget, actual: {Children.Length}", Children.Length >= 2);
			ReorderChild(widget, Children.Length - 2);
		}

		protected abstract void ResetToDefault();

		private void UpdateResetButton()
		{
			resetToDefaultsButton.Sensitive = !viewModel.IsDefault;
			resetToDefaultsButton.TooltipText = resetToDefaultsButton.Sensitive
				? ActiveResetTooltip
				: InactiveResetTooltip;
		}
	}
}
