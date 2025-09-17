using StackoverflowService.Application.DTOs.Answers;
using StackoverflowService.Application.DTOs.Users;
using System;
using System.Collections.Generic;

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
        
        //Agregation and navigation fields
        public UserPreviewDto User { get; set; } = new UserPreviewDto();
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
        public int VoteScore { get; set; } = default!;
    }
}
