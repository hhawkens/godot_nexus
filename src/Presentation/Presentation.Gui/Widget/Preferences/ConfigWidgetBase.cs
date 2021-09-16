using Gtk;

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
			Add(labelWidget);
		}
	}
}
