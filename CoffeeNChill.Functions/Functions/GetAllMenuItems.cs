// Reference: GeeksforGeeks, "Querying Azure Table Storage using C#,"
// GeeksforGeeks, 2023. [Online].
// Available: https://www.geeksforgeeks.org/.


using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions
{
    /// HTTP-triggered function that returns every menu item currently stored, across all categories.
    
    public class GetAllMenuItems
    {
        private readonly TableStorageService _tableService;

        public GetAllMenuItems(TableStorageService tableService)
        {
            _tableService = tableService;
        }

        [Function("GetAllMenuItems")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")] HttpRequestData req)
        {
            // No input since everything is fetched from storeage 
            var items = await _tableService.GetAllItemsAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(items);
            return response;
        }
    }
}