using StackoverflowService.Domain.ValueObjects;
using System.Threading.Tasks;
using System.Threading;

namespace StackoverflowService.Application.Abstractions
{
    public interface IPhotoStorage
    {
        Task<PhotoRef> UploadUserPhotoAsync(
            string userId,
            byte[] content,
            string contentType,
            string fileName,
            CancellationToken cancellationToken);
    }
}
