using System;

namespace StackoverflowService.Infrastructure.Configuration
{
    public class EmailApiOptions
    {
        public Uri BaseAddress { get; set; } = default!;
        public int TimeoutSeconds { get; set; } = 100;
        public string Token { get; set; } = string.Empty;
    }
}
