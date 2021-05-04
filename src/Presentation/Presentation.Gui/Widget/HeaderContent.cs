using Gtk;

namespace App.Presentation.Gui
{
	public class HeaderContent : Box
	{
		private const int Gap = 30;

		public HeaderContent(string text) : this() => TryAddLabel(text);

		public HeaderContent(IconInfo iconInfo, string? text = null) : this()
		{
			Add(iconInfo.CreateGtkImage());
			TryAddLabel(text);
		}

		public HeaderContent(ButtonContent buttonContent, string? text = null) : this()
		{
			Add(buttonContent.ToGtkButton());
			TryAddLabel(text);
		}

		private HeaderContent() : base(Orientation.Horizontal, Gap) { }

		private void TryAddLabel(string? text)
		{
			if (text is { } t)
			{
				var label = new Label(t);
				label.StyleContext.AddClass("heading");
				Add(label);
			}
		}
	}
}
