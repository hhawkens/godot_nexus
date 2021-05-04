namespace App.Presentation.Frontend
{
	public record Job<T>(string Description, JobStatus Status, T Data, IResult? Result);

	public enum JobStatus
	{
		WaitingForStart,
		Running,
		Finished
	}
}
