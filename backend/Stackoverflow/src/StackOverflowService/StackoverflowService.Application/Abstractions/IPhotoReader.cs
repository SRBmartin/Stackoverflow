using StackoverflowService.Application.DTOs.File;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Abstractions
{
    public interface IPhotoReader
    {
        Task<FileDownloadDto> DownloadAsync(string container, string blobName, CancellationToken cancellationToken);
    }
}
