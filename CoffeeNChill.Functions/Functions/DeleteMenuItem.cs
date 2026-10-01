// Reference: GeeksforGeeks, "Deleting Entities in Azure Table Storage using C#," GeeksforGeeks, 2023. [Online].
// Available:https://www.geeksforgeeks.org/. 

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions
{
    /// removes a selected menu item
    public class DeleteMenuItem
    {
        // injected using the singleton registered in Program.cs.
        private readonly TableStorageService _tableService;

        public DeleteMenuItem(TableStorageService tableService)
        {
            _tableService = tableService;
        }

        [Function("DeleteMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "menu/{category}/{id}")]
            HttpRequestData req,
            string category,
            string id)
        {
            // confirms if the item exists before deleting it
            var existing = await _tableService.GetItemAsync(category, id);
            if (existing is null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteAsJsonAsync(new { error = $"Menu item {id} in category {category} was not found." });
                return notFound;
            }

            await _tableService.DeleteItemAsync(category, id);

            // a 204 No Content will appear for a successful delete
            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
    }
}