// Reference: GeeksforGeeks, "Updating Entities in Azure Table Storage using C#," GeeksforGeeks, 2023. [Online].
// Available:https://www.geeksforgeeks.org/. 

using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions
{
    /// Updates an existing menu item's price and/or availability.

    public class UpdateMenuItem
    {
        private readonly TableStorageService _tableService;

        public UpdateMenuItem(TableStorageService tableService)
        {
            _tableService = tableService;
        }

        [Function("UpdateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "menu/{category}/{id}")]
            HttpRequestData req,
            string category,
            string id)
        {
            // confirm the item actually exists before attempting any update.
            var existing = await _tableService.GetItemAsync(category, id);
            if (existing is null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteAsJsonAsync(new { error = $"Menu item {id} in category {category} was not found." });
                return notFound;
            }

            // read and parse the request body.
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            MenuItemDto? dto;

            try
            {
                dto = JsonSerializer.Deserialize<MenuItemDto>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                var badJson = req.CreateResponse(HttpStatusCode.BadRequest);
                await badJson.WriteAsJsonAsync(new { error = "Request body is not valid JSON." });
                return badJson;
            }

            // validate the parsed data. Price is the only field with the rule that it can no tbe a negative value
            if (dto is null || dto.Price < 0)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Price cannot be negative." });
                return badRequest;
            }
            // apply the changes to the existing entity.
            existing.Price = dto.Price;
            existing.IsAvailable = dto.IsAvailable;
            if (!string.IsNullOrWhiteSpace(dto.Name)) existing.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description)) existing.Description = dto.Description;

            await _tableService.UpdateItemAsync(existing);

            // Return the full updated entity so the client can confirm exactly what was saved.
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(existing);
            return response;
        }
    }
}