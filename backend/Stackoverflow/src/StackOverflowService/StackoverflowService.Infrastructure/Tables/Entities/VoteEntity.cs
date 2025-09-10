using Azure;
using Azure.Data.Tables;
using System;

namespace StackoverflowService.Infrastructure.Tables.Entities
{
    public class VoteEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default; // answerId
        public string RowKey { get; set; } = default; // voteId

        public string AnswerId { get; set; } = default;
        public string UserId { get; set; } = default;

        public string Type { get; set; } = default; // "+" (upvote) or "-" (downvote)
        public DateTimeOffset CreationDate { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
