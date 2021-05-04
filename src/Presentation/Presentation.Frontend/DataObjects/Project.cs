using System.IO;

namespace App.Presentation.Frontend
{
	public record Project(
		string Name,
		bool IsFavorite,
		DirectoryInfo? CustomDirectory,
		EngineInstall? AssociatedEngine);
}
