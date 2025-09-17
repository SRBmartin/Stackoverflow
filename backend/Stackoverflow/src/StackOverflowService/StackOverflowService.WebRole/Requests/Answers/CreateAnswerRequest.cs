namespace StackOverflowService.WebRole.Requests.Answers
{
	public class CreateAnswerRequest
	{
        public string QuestionId { get; set; } = default!;
        public string Text { get; set; } = default!;
    }
}