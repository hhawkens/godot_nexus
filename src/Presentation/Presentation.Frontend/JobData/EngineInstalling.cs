namespace App.Presentation.Frontend
{
	public class EngineInstalling
	{
		public EngineInstallingAction Action { get; }
		public byte ProgressPercent { get; }
	}

	public enum EngineInstallingAction
	{
		Downloading,
		Installing
	}
}
