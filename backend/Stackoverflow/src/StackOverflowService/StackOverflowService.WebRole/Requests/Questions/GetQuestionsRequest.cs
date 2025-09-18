#nullable enable

namespace StackOverflowService.WebRole.Requests.Questions
{
	public class GetQuestionsRequest
	{
		public int Page { get; set; } = 1;
		public string? Title { get; set; } = string.Empty;
		public string SortBy { get; set; } = "date";
		public string Direction { get; set; } = "desc";
	}
}