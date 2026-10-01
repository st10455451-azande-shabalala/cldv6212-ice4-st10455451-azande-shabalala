// Reference: GeeksforGeeks, "Azure Table Storage - Create, Read, Update and
// Delete (CRUD) Operations using C#," GeeksforGeeks, 2023. [Online].
// Available: https://www.geeksforgeeks.org/.

using Azure;
using Azure.Data.Tables;

namespace CoffeeNChill.Functions.Models
{
    /// represents a single menu item stored in the Azure Table 'MenuItems'
    public class MenuItemEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;

        // displays the name of the menu item
        public string Name { get; set; } = default!;

        // a short description of the menu item shown on the menu
        public string Description { get; set; } = default!;

        public double Price { get; set; }

        // shows if a item is avalable to order
        public bool IsAvailable { get; set; }

        // timestamp and ETag are managed automatically by Azure Table Storage
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}