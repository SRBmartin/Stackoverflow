using System;

namespace StackoverflowService.Application.DTOs.Users
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Gender { get; set; } = ""; // "M", "F"
        public string State { get; set; } = "";
        public string City { get; set; } = "";
        public string Address { get; set; } = "";
        public string? PhotoBlobName { get; set; }
        public string? PhotoContainer { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }
}
