using System.Collections.Generic;
using System;

namespace StackoverflowService.Application.DTOs.Common
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    }
}