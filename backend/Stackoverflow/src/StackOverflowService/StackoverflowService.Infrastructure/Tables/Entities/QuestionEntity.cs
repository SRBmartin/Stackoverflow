using Azure;
using Azure.Data.Tables;
using System;

#nullable enable

namespace StackoverflowService.Infrastructure.Tables.Entities
{
    public class QuestionEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default!; // userId
        public string RowKey { get; set; } = default!; // questionId

        public string UserId { get; set; } = default!;

        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? PhotoBlobName { get; set; }
        public string? PhotoContainer { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
