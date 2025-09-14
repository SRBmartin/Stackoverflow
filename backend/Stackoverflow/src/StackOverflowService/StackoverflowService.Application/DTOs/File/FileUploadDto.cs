﻿using System;

namespace StackoverflowService.Application.DTOs.File
{
    public class FileUploadDto
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = "upload";
    }
}
