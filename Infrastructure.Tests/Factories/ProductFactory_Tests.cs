using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Xunit;

namespace Infrastructure.Tests.Factories
{
    public class ProductFactory_Tests
    {
        [Fact]
        public void ProductFactory_Create_ReturnsProductWithCorrectProperties()
        {
            // arrange
            var title = "Test Product";
            var price = 29.99m;

            // act
            var product = ProductFactory.Create(title, price);

            // assert
            Assert.NotNull(product);
            Assert.Equal(title, product.Title);
            Assert.Equal(price, product.Price);
            Assert.False(string.IsNullOrEmpty(product.Id));
        }

        [Fact]
        public void ProductFactory_Create_GeneratesUniqueIds()
        {
            // arrange
            var title = "Test Product";
            var price = 15.99m;

            // act
            var product1 = ProductFactory.Create(title, price);
            var product2 = ProductFactory.Create(title, price);

            // assert
            Assert.NotEqual(product1.Id, product2.Id);
            Assert.False(string.IsNullOrEmpty(product1.Id));
            Assert.False(string.IsNullOrEmpty(product2.Id));
        }

        [Fact]
        public void ProductFactory_Create_HighPrice_ReturnsValidProduct()
        {
            // arrange
            var title = "Expensive Product";
            var price = 9999.99m;

            // act
            var product = ProductFactory.Create(title, price);

            // assert
            Assert.NotNull(product);
            Assert.Equal(title, product.Title);
            Assert.Equal(price, product.Price);
            Assert.False(string.IsNullOrEmpty(product.Id));
        }

        [Fact]
        public void ProductFactory_Create_MinDecimalPrice_ReturnsValidProduct()
        {
            // arrange
            var title = "Min Price Product";
            var price = decimal.MinValue;

            // act
            var product = ProductFactory.Create(title, price);

            // assert
            Assert.NotNull(product);
            Assert.Equal(title, product.Title);
            Assert.Equal(decimal.MinValue, product.Price);
            Assert.False(string.IsNullOrEmpty(product.Id));
        }
    }
}
