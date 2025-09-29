using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Xunit;

namespace Infrastructure.Tests.Factories
{
    public class ProductFactory_Tests
    {
        private Category GetTestCategory() => new Category { Name = "Test Category" };
        private Manufacturer GetTestManufacturer() => new Manufacturer { Name = "Test Manufacturer" };

        [Fact]
        public void ProductFactory_Create_ReturnsProductWithCorrectProperties()
        {
            // arrange
            var title = "Test Product";
            var price = 29.99m;
            var category = GetTestCategory();
            var manufacturer = GetTestManufacturer();

            // act
            var product = ProductFactory.Create(title, price, category, manufacturer);

            // assert
            Assert.NotNull(product);
            Assert.Equal(title, product.Title);
            Assert.Equal(price, product.Price);
            Assert.False(string.IsNullOrEmpty(product.Id));
            Assert.Equal(category, product.Category);
            Assert.Equal(manufacturer, product.Manufacturer);
        }

        [Fact]
        public void ProductFactory_Create_GeneratesUniqueIds()
        {
            // arrange
            var title = "Test Product";
            var price = 15.99m;
            var category = GetTestCategory();
            var manufacturer = GetTestManufacturer();

            // act
            var product1 = ProductFactory.Create(title, price, category, manufacturer);
            var product2 = ProductFactory.Create(title, price, category, manufacturer);

            // assert
            Assert.NotEqual(product1.Id, product2.Id);
            Assert.False(string.IsNullOrEmpty(product1.Id));
            Assert.False(string.IsNullOrEmpty(product2.Id));
            Assert.Equal(category, product1.Category);
            Assert.Equal(manufacturer, product1.Manufacturer);
            Assert.Equal(category, product2.Category);
            Assert.Equal(manufacturer, product2.Manufacturer);
        }

        [Fact]
        public void ProductFactory_Create_HighPrice_ReturnsValidProduct()
        {
            // arrange
            var title = "Expensive Product";
            var price = 9999.99m;
            var category = GetTestCategory();
            var manufacturer = GetTestManufacturer();

            // act
            var product = ProductFactory.Create(title, price, category, manufacturer);

            // assert
            Assert.NotNull(product);
            Assert.Equal(title, product.Title);
            Assert.Equal(price, product.Price);
            Assert.False(string.IsNullOrEmpty(product.Id));
            Assert.Equal(category, product.Category);
            Assert.Equal(manufacturer, product.Manufacturer);
        }

        [Fact]
        public void ProductFactory_Create_MinDecimalPrice_ReturnsValidProduct()
        {
            // arrange
            var title = "Min Price Product";
            var price = decimal.MinValue;
            var category = GetTestCategory();
            var manufacturer = GetTestManufacturer();

            // act
            var product = ProductFactory.Create(title, price, category, manufacturer);

            // assert
            Assert.NotNull(product);
            Assert.Equal(title, product.Title);
            Assert.Equal(decimal.MinValue, product.Price);
            Assert.False(string.IsNullOrEmpty(product.Id));
            Assert.Equal(category, product.Category);
            Assert.Equal(manufacturer, product.Manufacturer);
        }
    }
}
