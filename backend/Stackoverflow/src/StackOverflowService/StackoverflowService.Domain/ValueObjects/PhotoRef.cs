namespace StackoverflowService.Domain.ValueObjects
{
    public sealed class PhotoRef
    {
        public string BlobName { get; }
        public string Container { get; }

        public PhotoRef(string blobName, string container)
        {
            BlobName = blobName;
            Container = container;
        }

    }
}
