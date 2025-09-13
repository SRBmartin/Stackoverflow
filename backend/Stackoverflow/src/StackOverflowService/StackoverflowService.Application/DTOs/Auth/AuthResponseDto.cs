using System;

namespace StackoverflowService.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public string TokenType { get; set; } = "Bearer";
        public DateTimeOffset ExpiresAt { get; set; }
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Lastname { get; set; } = default!;
    }
}
