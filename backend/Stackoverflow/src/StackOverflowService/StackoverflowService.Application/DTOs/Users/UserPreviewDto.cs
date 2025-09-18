#nullable enable

namespace StackoverflowService.Application.DTOs.Users
{
    public class UserPreviewDto
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhotoBlobName { get; set; }
        public string? PhotoContainer { get; set; }
    }
}
