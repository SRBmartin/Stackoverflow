using System;

namespace StackoverflowService.Application.DTOs.Answers
{
    public class AnswerDto
    {
        public string Id { get; set; } = default!;
        public string QuestionId { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Text { get; set; } = default!;
        public DateTimeOffset CreationDate { get; set; }
        public bool IsFinal { get; set; }
        public bool IsDeleted { get; set; }

        //Aggregations
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int VoteScore { get; set; }
    }
}
