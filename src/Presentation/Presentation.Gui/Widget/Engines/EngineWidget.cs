using Gtk;
using static App.Presentation.Gui.Styling;

namespace App.Presentation.Gui;

public class EngineWidget : Box
{
	public EngineWidget() : base(Orientation.Horizontal, TopLevelSpacing)
	{
		StyleContext.AddClass("border-square");
		Homogeneous = true;
		MarginStart = TopLevelSpacing;
		MarginEnd = TopLevelSpacing;
		Add(StartBox());
		Add(EndBox());
	}

	private static Box StartBox()
	{
		var box = new Box(Orientation.Horizontal, TopLevelSpacing);

		var engineIconWidget = new IconInfo(IconType.GodotMonochrome, ThemeTones.PresetThemeTone).CreateGtkImage();
		AddPaddingFromLeftmost(engineIconWidget);
		box.Add(engineIconWidget);

		box.Add(StartBoxLabel("3.3.4"));
		box.Add(StartBoxLabel(".NET"));

		return box;
	}

	private static Box EndBox()
	{
		var box = new Box(Orientation.Horizontal, TopLevelSpacing);
		box.Halign = Align.End;

		var openEngineIcon = new IconInfo(IconType.Goto, ThemeTones.PresetThemeTone);
		var openEngineButton = new ButtonContent(openEngineIcon, delegate { }).ToGtkButton();
		openEngineButton.TooltipText = "Open";
		openEngineButton.Valign = Align.Center;
		openEngineButton.MarginEnd = SubLevelSpacing;
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
		widget.MarginTop = SubLevelSpacing;
		widget.MarginBottom = SubLevelSpacing;
		widget.MarginStart = SubLevelSpacing;
	}
}
