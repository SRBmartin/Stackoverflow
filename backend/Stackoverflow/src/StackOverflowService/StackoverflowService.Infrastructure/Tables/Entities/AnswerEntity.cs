using Azure;
using Azure.Data.Tables;
using System;

namespace StackoverflowService.Infrastructure.Tables.Entities
{
    public class AnswerEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default; // questionId
        public string RowKey { get; set; } = default; // answerId

        public string QuestionId { get; set; } = default;
        public string Text { get; set; } = default;
        public DateTimeOffset CreationDate { get; set; }
        public bool IsFinal { get; set; }
        public bool IsDeleted { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
