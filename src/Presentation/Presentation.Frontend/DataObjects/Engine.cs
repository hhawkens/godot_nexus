using System;
using System.IO;

namespace App.Presentation.Frontend
{
	public record EngineInstall(DirectoryInfo Directory, Version Version, EngineAttributes Attributes)
		: Engine(Version, Attributes);

	public record Engine(Version Version, EngineAttributes Attributes);

	[Flags]
	public enum EngineAttributes
	{
		Mono = 0x1
	}
}
