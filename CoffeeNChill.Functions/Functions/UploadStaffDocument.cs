using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions
{
    /// uploads a staff document to Blob Storage.
    public class UploadStaffDocument
    {
        private readonly BlobStorageService _blobService;
        private readonly ILogger<UploadStaffDocument> _logger;
        private static readonly string[] AllowedExtensions = { ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx" };

        public UploadStaffDocument(BlobStorageService blobService, ILogger<UploadStaffDocument> logger)
        {
            _blobService = blobService;
            _logger = logger;
        }

        [Function("UploadStaffDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents/upload")] HttpRequest req)
        {
            _logger.LogInformation("Processing request to upload staff document.");

            if (!req.HasFormContentType || req.Form.Files.Count == 0)
            {
                return new BadRequestObjectResult("Please upload a file using form-data.");
            }

            var file = req.Form.Files[0];
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            {
                return new BadRequestObjectResult($"File type '{extension}' is not permitted.");
            }

            using var stream = file.OpenReadStream();
            await _blobService.UploadDocumentAsync(stream, file.FileName, file.ContentType);

            return new OkObjectResult(new
            {
                message = $"File '{file.FileName}' uploaded successfully.",
                sizeInBytes = file.Length
            });
        }
    }
}