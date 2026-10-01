using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Services;
using Xunit;

namespace CoffeeNChill.Functions.Tests
{
    public class MenuItemValidatorTests
    {
        [Fact]
        public void IsValid_ReturnsTrue_ForCompleteValidItem()
        {
            var dto = new MenuItemDto
            {
                Category = "Hot Drinks",
                Sku = "COF-001",
                Name = "Espresso",
                Description = "Double shot espresso",
                Price = 25.00,
                IsAvailable = true
            };

            var result = MenuItemValidator.IsValid(dto, out var error);

            Assert.True(result);
            Assert.Equal(string.Empty, error);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenSkuIsMissing()
        {
            var dto = new MenuItemDto
            {
                Category = "Hot Drinks",
                Sku = "",
                Name = "Espresso",
                Price = 25.00
            };

            var result = MenuItemValidator.IsValid(dto, out var error);

            Assert.False(result);
            Assert.False(string.IsNullOrWhiteSpace(error));
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenPriceIsNegative()
        {
            var dto = new MenuItemDto
            {
                Category = "Hot Drinks",
                Sku = "COF-001",
                Name = "Espresso",
                Price = -5.00
            };

            var result = MenuItemValidator.IsValid(dto, out var error);

            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenDtoIsNull()
        {
            var result = MenuItemValidator.IsValid(null, out var error);

            Assert.False(result);
        }
    }
}