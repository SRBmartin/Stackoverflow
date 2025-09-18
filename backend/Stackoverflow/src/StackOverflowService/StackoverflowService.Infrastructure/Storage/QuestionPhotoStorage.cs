using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Domain.ValueObjects;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace StackoverflowService.Infrastructure.Storage
{
    public class QuestionPhotoStorage : IQuestionPhotoStorage
    {
        private readonly BlobContainerClient _container;
        private readonly string _containerName;

        public QuestionPhotoStorage()
        {
            var connectionString = StorageConnection.Get();
            _containerName = ConfigurationManager.AppSettings["QuestionPhotosContainer"] ?? "questionphotos";

            _container = new BlobContainerClient(connectionString, _containerName);

            try
            {
                _container.CreateIfNotExists(PublicAccessType.Blob);
            }
            catch (RequestFailedException)
            {
                //Ignore races / emulator transients
            }
        }

        public async Task<PhotoRef> UploadQuestionPhotoAsync(string userId, string questionId, byte[] content, string contentType, string fileName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(questionId)) throw new ArgumentNullException(nameof(questionId));
            if (content == null || content.Length == 0) throw new ArgumentException("Empty content.", nameof(content));
            if (string.IsNullOrWhiteSpace(contentType)) contentType = "application/octet-stream";

            var safeName = MakeSafeFileName(fileName, contentType);
            var blobName = $"questions/{userId}/{questionId}/{Guid.NewGuid():N}-{safeName}";

            var blob = _container.GetBlobClient(blobName);
            var headers = new BlobHttpHeaders { ContentType = contentType };

            using (var ms = new MemoryStream(content, writable: false))
            {
                var options = new BlobUploadOptions { HttpHeaders = headers };
                await blob.UploadAsync(ms, options, cancellationToken);
            }

            return new PhotoRef(blobName, _containerName);
        }

        #region Helpers
        private static string MakeSafeFileName(string fileName, string contentType)
        {
            var ext = GetExtension(fileName, contentType);
            var baseName = string.IsNullOrWhiteSpace(fileName)
                ? "photo"
                : Path.GetFileNameWithoutExtension(fileName).Trim();

            if (baseName.Length == 0) baseName = "photo";

            baseName = Regex.Replace(baseName, @"\s+", "-");
            baseName = Regex.Replace(baseName, @"[^A-Za-z0-9\-_]", "");
            if (baseName.Length > 64) baseName = baseName.Substring(0, 64);

            return baseName + ext;
        }

        private static string GetExtension(string fileName, string contentType)
        {
            var ext = Path.GetExtension(fileName ?? "");
            if (!string.IsNullOrEmpty(ext)) return ext.ToLowerInvariant();

            switch ((contentType ?? "").ToLowerInvariant())
            {
                case "image/jpeg": return ".jpg";
                case "image/jpg": return ".jpg";
                case "image/png": return ".png";
                case "image/gif": return ".gif";
                case "image/webp": return ".webp";
                default: return ".bin";
            }
        }
        #endregion

    }
}
