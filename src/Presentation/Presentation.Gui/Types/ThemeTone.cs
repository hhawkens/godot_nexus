using Gtk;

namespace App.Presentation.Gui
{
	/// Describes the color theme of the GUI.
	public enum ThemeTone
	{
		Lighter,
		Darker
	}

	/// Provides helper methods with regards to GUI theming.
	public static class ThemeTones
	{
		/// Determines whether the currently used theme is light or dark. Uses a widget as a sample.
		public static ThemeTone GetCurrentThemeTone(this Widget widget)
		{
#pragma warning disable 612
			// Method is deprecated, but there is no replacement (yet?)
			var baseColor = widget.StyleContext.GetBackgroundColor(StateFlags.Normal);
#pragma warning restore 612

			var grayScale = (baseColor.Red + baseColor.Green + baseColor.Blue) / 3.0;
			return grayScale < 0.5 ? ThemeTone.Darker : ThemeTone.Lighter;
		}
	}
}
