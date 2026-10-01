using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions
{
    /// Downloads a single staff document from Blob Storage.
    public class DownloadStaffDocument
    {
        private readonly BlobStorageService _blobService;
        private readonly ILogger<DownloadStaffDocument> _logger;

        public DownloadStaffDocument(BlobStorageService blobService, ILogger<DownloadStaffDocument> logger)
        {
            _blobService = blobService;
            _logger = logger;
        }

        [Function("DownloadStaffDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents/download/{fileName}")] HttpRequest req,
            string fileName)
        {
            _logger.LogInformation("Processing request to download staff document.");

            if (string.IsNullOrEmpty(fileName))
            {
                return new BadRequestObjectResult("Please provide a filename.");
            }

            var result = await _blobService.DownloadDocumentAsync(fileName);
            if (result is null)
            {
                return new NotFoundObjectResult($"File '{fileName}' not found.");
            }

            var (content, contentType) = result.Value;
            return new FileStreamResult(content, contentType) { FileDownloadName = fileName };
        }
    }
}