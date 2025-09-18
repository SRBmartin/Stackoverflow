namespace StackoverflowService.Application.DTOs.File
{
    public class FileDownloadDto
    {
        public byte[] Content { get; set; } = default!;
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = "file";
    }
}
