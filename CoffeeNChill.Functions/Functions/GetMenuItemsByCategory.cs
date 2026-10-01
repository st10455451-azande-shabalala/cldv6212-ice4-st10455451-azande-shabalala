// Reference: GeeksforGeeks, "Filtering Data in Azure Table Storage with PartitionKey," GeeksforGeeks, 2023. [Online].
// Available:https://www.geeksforgeeks.org/. 

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions
{
    /// HTTP-triggered function that returns only the menu items belonging to a specific category.

    public class GetMenuItemsByCategory
    {
        private readonly TableStorageService _tableService;

        public GetMenuItemsByCategory(TableStorageService tableService)
        {
            _tableService = tableService;
        }

        [Function("GetMenuItemsByCategory")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu/category/{category}")]
            HttpRequestData req,
            string category)
        {
            // Check the category was actually provided.
            if (string.IsNullOrWhiteSpace(category))
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Category route parameter is required." });
                return badRequest;
            }

            var items = await _tableService.GetItemsByCategoryAsync(category);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(items);
            return response;
        }
    }
}