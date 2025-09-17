namespace StackOverflowService.WebRole.Requests.Questions
{
	public class CreateQuestionRequest
	{
		public string Title { get; set; } = default!;
		public string Description { get; set; } = default!;
	}
}