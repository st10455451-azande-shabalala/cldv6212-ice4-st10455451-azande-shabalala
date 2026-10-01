namespace CoffeeNChill.Functions.Models
{
    /// Represents a menu item as sent to or received from the API.
    /// Keeps the API separate from how data is stored in Azure Table Storage.
    public class MenuItemDto
    {
        // the category of the item
        public string Category { get; set; } = default!;

        // the unique code for each item 
        public string Sku { get; set; } = default!;

        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}