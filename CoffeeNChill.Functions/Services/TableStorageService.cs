// Reference: GeeksforGeeks, "Working with Azure Table Storage using C#,"
// GeeksforGeeks, 2023. [Online]. Available: https://www.geeksforgeeks.org/.
// Used as a reference for the TableClient query and CRUD method patterns below.

using Azure.Data.Tables;
using CoffeeNChill.Functions.Models;

namespace CoffeeNChill.Functions.Services
{
    /// Handles all interaction with the "MenuItems" Azure Table.
    public class TableStorageService
    {
        private readonly TableClient _tableClient;
        public TableStorageService(string connectionString)
        {
            _tableClient = new TableClient(connectionString, "MenuItems");

            // CreateIfNotExists makes sure the MenuItems table exists the first time the app runs, so we don't have to create it manually in Azurite before testing.
            _tableClient.CreateIfNotExists();
        }

        /// inserts a new menu item entity into the MenuItems table.
        public async Task AddItemAsync(MenuItemEntity entity)
        {
            await _tableClient.AddEntityAsync(entity);
        }

        /// returns all menu items stored in the MenuItems table 
        public async Task<List<MenuItemEntity>> GetAllItemsAsync()
        {
            var items = new List<MenuItemEntity>();

            // QueryAsync streams results page by page
            await foreach (var item in _tableClient.QueryAsync<MenuItemEntity>())
            {
                items.Add(item);
            }

            return items;
        }

        /// returns only the menu items belonging to a specific category.
        public async Task<List<MenuItemEntity>> GetItemsByCategoryAsync(string category)
        {
            var items = new List<MenuItemEntity>();

            await foreach (var item in _tableClient.QueryAsync<MenuItemEntity>(
                x => x.PartitionKey == category))
            {
                items.Add(item);
            }

            return items;
        }

        /// retrieves a single menu item by its category and SKU
        public async Task<MenuItemEntity?> GetItemAsync(string category, string sku)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<MenuItemEntity>(category, sku);
                return response.Value;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                // Azure Table Storage throws a 404 RequestFailedException when the entity isn't found
                return null;
            }
        }

        /// updates an existing menu item.
        public async Task UpdateItemAsync(MenuItemEntity entity)
        {
            await _tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
        }

        /// permanently removes a menu item from the table.
        public async Task DeleteItemAsync(string category, string sku)
        {
            await _tableClient.DeleteEntityAsync(category, sku);
        }
    }
}