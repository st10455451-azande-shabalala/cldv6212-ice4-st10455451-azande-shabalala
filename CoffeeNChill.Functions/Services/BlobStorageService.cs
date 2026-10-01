// Reference: GeeksforGeeks, "Uploading and Downloading Blobs in Azure Blob
// Storage using C#," GeeksforGeeks, 2023. [Online]. Available:
// https://www.geeksforgeeks.org/. Used as a reference for the BlobClient
// upload/download pattern below.

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CoffeeNChill.Functions.Models;

namespace CoffeeNChill.Functions.Services
{
    /// Handles all interaction with the "staff-docs" Blob Storage container.
    public class BlobStorageService
    {
        private const string ContainerName = "staff-docs";
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task UploadDocumentAsync(Stream content, string fileName, string? contentType)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType ?? "application/octet-stream" }
            };
            await blobClient.UploadAsync(content, uploadOptions);
        }

        public async Task<List<StaffDocumentInfo>> ListDocumentsAsync()
        {
            var documents = new List<StaffDocumentInfo>();
            await foreach (var blobItem in _containerClient.GetBlobsAsync())
            {
                documents.Add(new StaffDocumentInfo
                {
                    FileName = blobItem.Name,
                    SizeInBytes = blobItem.Properties.ContentLength ?? 0,
                    LastModified = blobItem.Properties.LastModified
                });
            }
            return documents;
        }

        public async Task<(Stream Content, string ContentType)?> DownloadDocumentAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            if (!await blobClient.ExistsAsync())
            {
                return null;
            }
            var download = await blobClient.DownloadStreamingAsync();
            return (download.Value.Content, download.Value.Details.ContentType ?? "application/octet-stream");
        }
    }
}