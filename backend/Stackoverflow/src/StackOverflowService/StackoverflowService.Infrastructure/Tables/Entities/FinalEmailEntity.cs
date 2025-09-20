using Azure;
using Azure.Data.Tables;
using System;

namespace StackoverflowService.Infrastructure.Tables.Entities
{
    public class FinalEmailEntity : ITableEntity
    {
        public string PartitionKey { get; set; } // AnswerId
        public string RowKey { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public int SentCount { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
