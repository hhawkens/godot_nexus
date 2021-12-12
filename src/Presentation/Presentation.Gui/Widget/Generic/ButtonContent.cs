using System;
using Gtk;

namespace App.Presentation.Gui;

public class ButtonContent
{
	private readonly IconInfo? iconInfo;
	private readonly string? text;
	private readonly EventHandler clickAction;

	public ButtonContent(IconInfo iconInfo, EventHandler clickAction) : this(clickAction)
	{
		this.iconInfo = iconInfo;
		text = null;
	}

	public ButtonContent(string text, EventHandler clickAction) : this(clickAction)
	{
		this.text = text;
		iconInfo = null;
	}

	private ButtonContent(EventHandler clickAction) => this.clickAction = clickAction;

	public Button ToGtkButton()
	{
		var button = new Button();
		button.Clicked += clickAction;

		if (iconInfo is {} ic)
		{
			button.Add(ic.CreateGtkImage());
			button.Relief = ReliefStyle.None;
			button.StyleContext.AddClass("icon-button");
		}
		else if (text is {} txt)
		{
			button.Label = txt;
		}

		return button;
	}
}
