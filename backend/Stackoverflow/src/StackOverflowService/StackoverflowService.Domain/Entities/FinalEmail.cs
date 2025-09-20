using System;

namespace StackoverflowService.Domain.Entities
{
    public class FinalEmail
    {
        public string AnswerId { get; }
        public int SentCount { get; }
        public DateTimeOffset CreatedAt { get; }

        public FinalEmail(string answerId, int sentCount, DateTimeOffset createdAt)
        {
            AnswerId = answerId ?? throw new ArgumentNullException(nameof(answerId));
            SentCount = sentCount;
            CreatedAt = createdAt;
        }

    }
}
