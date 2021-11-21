using Gtk;

namespace App.Presentation.Gui
{
	public class EngineWidget : Box
	{
		private const int SpacingPixels = 16;
		private const int WidgetPadding = 6;

		public EngineWidget() : base(Orientation.Horizontal, SpacingPixels)
		{
			StyleContext.AddClass("border-square");
			Homogeneous = true;
			Add(StartBox());
			Add(EndBox());
		}

		private static Box StartBox()
		{
			var box = new Box(Orientation.Horizontal, SpacingPixels);

			var engineIconWidget = new IconInfo(IconType.GodotMonochrome, ThemeTones.PresetThemeTone).CreateGtkImage();
			AddPaddingFromLeftmost(engineIconWidget);
			box.Add(engineIconWidget);

			box.Add(StartBoxLabel("3.3.4"));
			box.Add(StartBoxLabel(".NET"));

			return box;
		}

		private static Box EndBox()
		{
			var box = new Box(Orientation.Horizontal, SpacingPixels);
			box.Halign = Align.End;

			var openEngineIcon = new IconInfo(IconType.Goto, ThemeTones.PresetThemeTone);
			var openEngineButton = new ButtonContent(openEngineIcon, delegate { }).ToGtkButton();
			openEngineButton.TooltipText = "Open";
			openEngineButton.Valign = Align.Center;
			openEngineButton.MarginEnd = WidgetPadding;
			box.Add(openEngineButton);

			return box;
		}

		private static Label StartBoxLabel(string text)
		{
			var label = Label.New(text);
			label.Xalign = 0;
			label.StyleContext.AddClass("heading-light");
			return label;
		}

		private static void AddPaddingFromLeftmost(Widget widget)
		{
			widget.MarginTop = WidgetPadding;
			widget.MarginBottom = WidgetPadding;
			widget.MarginStart = WidgetPadding;
		}
	}
}
