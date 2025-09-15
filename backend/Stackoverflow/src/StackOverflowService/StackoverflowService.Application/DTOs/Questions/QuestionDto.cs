using StackoverflowService.Application.DTOs.Users;
using System;

namespace StackoverflowService.Application.DTOs.Questions
{
    public class QuestionDto
    {
        public string Id { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? PhotoBlobName { get; set; }
        public string? PhotoContainer { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }

        
        public UserPreviewDto User { get; set; } = new UserPreviewDto();
    }
}
