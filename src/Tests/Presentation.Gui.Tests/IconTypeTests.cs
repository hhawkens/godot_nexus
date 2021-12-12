using FSharpPlus;
using NUnit.Framework;

namespace App.Presentation.Gui.Tests;

[TestFixture]
public class IconTypeTests
{
	[Test]
	public void GetIconPath_EachListEntry_Exists()
	{
		var allListEntryImages = Enums.iterate<IconType>();
		var allThemes = Enums.iterate<ThemeTone>();

		foreach (var iconType in allListEntryImages)
		{
			foreach (var theme in allThemes)
			{
				Assert.That(iconType.GetIconPath(theme), Is.Not.Null, $"{nameof(IconType)}.{iconType}");
				Assert.That(iconType.GetIconPath(theme), Is.Not.Empty, $"{nameof(IconType)}.{iconType}");
			}
		}
	}
}
