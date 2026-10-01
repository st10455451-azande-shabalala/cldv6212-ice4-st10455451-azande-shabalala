// Reference: GeeksforGeeks, "HTTP Trigger in Azure Functions using C#," GeeksforGeeks, 2023. [Online].
// Available: https://www.geeksforgeeks.org/.


using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions
{
    /// HTTP-triggered function that creates a new menu item.

    public class CreateMenuItem
    {
        // injected using the singleton registered in Program.cs.
        private readonly TableStorageService _tableService;

        public CreateMenuItem(TableStorageService tableService)
        {
            _tableService = tableService;
        }

        [Function("CreateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequestData req)
        {
            // reads the request as a string and parses it and returns a clear error if it is malformed
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            MenuItemDto? dto;

            try
            {
                dto = JsonSerializer.Deserialize<MenuItemDto>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                // return 400 if body was not a valid JSON
                var badJson = req.CreateResponse(HttpStatusCode.BadRequest);
                await badJson.WriteAsJsonAsync(new { error = "Request body is not valid JSON." });
                return badJson;
            }

            // valadtion used to check that are fields are completed and correct before storing it
            if (dto is null
                || string.IsNullOrWhiteSpace(dto.Category)
                || string.IsNullOrWhiteSpace(dto.Sku)
                || string.IsNullOrWhiteSpace(dto.Name)
                || dto.Price < 0)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new
                {
                    error = "Category, Sku, and Name are required, and Price cannot be a negative value"
                });
                return badRequest;
            }

            // map the client facing DTO onto the storage entity
            var entity = new MenuItemEntity
            {
                PartitionKey = dto.Category,
                RowKey = dto.Sku,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                IsAvailable = dto.IsAvailable
            };

            await _tableService.AddItemAsync(entity);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(dto);
            return response;
        }
    }
}