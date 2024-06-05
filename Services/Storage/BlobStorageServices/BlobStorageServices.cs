using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SchedulerApi.Services.Storage.BlobStorageServices;

public class BlobStorageServices : IBlobStorageServices
{
    private readonly BlobServiceClient _blobServiceClient;
    
    public BlobStorageServices(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> StoreAsync(Stream stream, string containerName, string blobName, string contentType = "application/octet-stream")
    {
        var blob = GetBlobClient(containerName, blobName);
        await blob.UploadAsync(stream, overwrite: true);

        var httpHeaders = new BlobHttpHeaders
        {
            ContentType = contentType
        };
        await blob.SetHttpHeadersAsync(httpHeaders);
        return blob.Uri.ToString();
    }

    public async Task<string> StoreAsync(string stringContent, string containerName, string blobName, string contentType = "application/octet-stream")
    {
        var stream = ConvertToStream(stringContent);
        return await StoreAsync(stream, containerName, blobName, contentType);
    }

    public string GetBlobUrl(string containerName, string blobName)
    {
        return _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName)
            .Uri
            .ToString();
    }

    private BlobClient GetBlobClient(string containerName, string blobName)
    {
        return _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);
    }
    
    private Stream ConvertToStream(string stringContent)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(stringContent));
    }
}
