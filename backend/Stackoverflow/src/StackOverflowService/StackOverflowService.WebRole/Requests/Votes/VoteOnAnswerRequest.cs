#nullable enable

namespace StackOverflowService.WebRole.Requests.Votes
{
	public class VoteOnAnswerRequest
	{
		public string Type { get; set; } = ""; //'+' or '-'
		public string QuestionId { get; set; }
		public string? AnswerId { get; set; } 
	}
}