using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Xunit;

namespace Infrastructure.Tests.Factories
{
    public class ProductFactory_Tests
    {
        private Category GetTestCategory() => new Category { Name = "Clothes" };
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
            Assert.Equal(category.Name, product.Category.Name);
            Assert.Equal(manufacturer.Name, product.Manufacturer.Name);
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
            Assert.Equal(category.Name, product1.Category.Name);
            Assert.Equal(manufacturer.Name, product1.Manufacturer.Name);
            Assert.Equal(category.Name, product2.Category.Name);
            Assert.Equal(manufacturer.Name, product2.Manufacturer.Name);
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
            Assert.Equal(category.Name, product.Category.Name);
            Assert.Equal(manufacturer.Name, product.Manufacturer.Name);
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
            Assert.Equal(category.Name, product.Category.Name);
            Assert.Equal(manufacturer.Name, product.Manufacturer.Name);
        }
    }
}
