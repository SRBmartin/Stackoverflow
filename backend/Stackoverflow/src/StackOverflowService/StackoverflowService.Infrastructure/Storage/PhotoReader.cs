using Azure;
using Azure.Storage.Blobs;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.DTOs.File;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace StackoverflowService.Infrastructure.Storage
{
    public class PhotoReader : IPhotoReader
    {
        private readonly BlobServiceClient _blobService;

        public PhotoReader()
        {
            var connectionString = StorageConnection.Get();
            _blobService = new BlobServiceClient(connectionString);
        }

        public async Task<FileDownloadDto> DownloadAsync(string container, string blobName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(blobName)) throw new ArgumentNullException(nameof(blobName));

            var containerClient = _blobService.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(blobName);

            try
            {
                var resp = await blobClient.DownloadContentAsync(cancellationToken);
                var contentType = resp.Value.Details.ContentType ?? "application/octet-stream";
                var fileName = Path.GetFileName(blobName);

                return new FileDownloadDto
                {
                    Content = resp.Value.Content.ToArray(),
                    ContentType = contentType,
                    FileName = fileName
                };
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                throw new FileNotFoundException($"Image not found: {container}/{blobName}");
            }
        }

    }
}
