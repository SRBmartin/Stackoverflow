using Azure;
using Azure.Data.Tables;
using System;

namespace StackoverflowService.Infrastructure.Tables.Entities
{
    public class UserEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "USR";
        public string RowKey { get; set; } = default;

        public string Name { get; set; } = default;
        public string Lastname { get; set; } = default;
        public DateTimeOffset CreationDate { get; set; }
        public string Email { get; set; } = default;
        public string Gender { get; set; } = default; // "M" "F"
        public string State { get; set; } = default;
        public string City { get; set; } = default;
        public string Address { get; set; } = default;
        public string PasswordHash { get; set; } = default;
        public string PhotoBlobName { get; set; } = default;
        public string PhotoContainer { get; set; } = default;

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
