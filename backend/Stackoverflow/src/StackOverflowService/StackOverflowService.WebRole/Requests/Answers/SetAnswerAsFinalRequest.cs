namespace StackOverflowService.WebRole.Requests.Answers
{
	public class SetAnswerAsFinalRequest
	{
        public string QuestionId { get; set; } = default!;
        public string AnswerId { get; set; } = default!;
    }
}