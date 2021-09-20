using Gtk;
using App.Utilities;

namespace App.Presentation.Gui
{
	public abstract class ConfigWidgetBase : Box
	{
		protected const int ElementWidth = 250;
		private const int SpacingPixels = 16;
		private const int LabelStartMargin = 50;

		protected ConfigWidgetBase(string label, string labelTooltip)
			: base(Orientation.Horizontal, SpacingPixels)
		{
			var labelWidget = Label.New(label);
			labelWidget.TooltipText = labelTooltip;
			labelWidget.MarginStart = LabelStartMargin;
			labelWidget.Xalign = 0;
			labelWidget.SetSizeRequest(ElementWidth, labelWidget.AllocatedHeight);
			base.Add(labelWidget);

			var theme = ThemeTones.PresetThemeTone;
			var resetToDefaultsButton = new ButtonContent(new IconInfo(IconType.Reset, theme), delegate {  });
			base.Add(resetToDefaultsButton.ToGtkButton());
		}

		/// We always want new widgets added by inheritors to be placed BEFORE the last one,
		/// which we add in this class.
		protected new void Add(Widget widget)
		{
			base.Add(widget);
			Operators.assertThat($"Expected >2 children in widget, actual: {Children.Length}", Children.Length >= 2);
			ReorderChild(widget, Children.Length - 2);
		}
	}
}
