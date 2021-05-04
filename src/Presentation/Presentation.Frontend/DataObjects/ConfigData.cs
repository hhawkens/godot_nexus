namespace App.Presentation.Frontend
{
	public record ConfigData<T>(string Description, T DefaultValue, T Value);
}
