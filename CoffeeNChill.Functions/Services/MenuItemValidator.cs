using CoffeeNChill.Functions.Models;

namespace CoffeeNChill.Functions.Services
{
    /// <summary>
    /// Pure validation logic for a MenuItemDto, extracted from CreateMenuItem
    /// so it can be unit tested without needing a real HTTP request.
    /// </summary>
    public static class MenuItemValidator
    {
        public static bool IsValid(MenuItemDto? dto, out string error)
        {
            if (dto is null
                || string.IsNullOrWhiteSpace(dto.Category)
                || string.IsNullOrWhiteSpace(dto.Sku)
                || string.IsNullOrWhiteSpace(dto.Name)
                || dto.Price < 0)
            {
                error = "Category, Sku, and Name are required, and Price cannot be a negative value";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}