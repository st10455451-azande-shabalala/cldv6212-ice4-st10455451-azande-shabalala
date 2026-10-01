using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions
{
    /// lists all staff documents stored in Blob Storage.
    public class ListStaffDocuments
    {
        private readonly BlobStorageService _blobService;
        private readonly ILogger<ListStaffDocuments> _logger;

        public ListStaffDocuments(BlobStorageService blobService, ILogger<ListStaffDocuments> logger)
        {
            _blobService = blobService;
            _logger = logger;
        }

        [Function("ListStaffDocuments")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")] HttpRequest req)
        {
            _logger.LogInformation("Processing request to list staff documents.");
            var documents = await _blobService.ListDocumentsAsync();
            return new OkObjectResult(documents);
        }
    }
}