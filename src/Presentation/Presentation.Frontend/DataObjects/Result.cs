namespace App.Presentation.Frontend
{
	public interface IResult
	{
		ResultValue Value { get; }
	}

	public enum ResultValue
	{
		Ok,
		Error
	}

	public record Ok : IResult
	{
		public ResultValue Value => ResultValue.Ok;
	}

	public record Error(string Message) : IResult
	{
		public ResultValue Value => ResultValue.Error;
	}
}
