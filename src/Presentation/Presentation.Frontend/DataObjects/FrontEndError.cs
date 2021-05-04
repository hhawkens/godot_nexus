namespace App.Presentation.Frontend
{
	public record FrontEndError(FrontendErrorType Type, string Message);

	public enum FrontendErrorType
	{
		General
	}
}
