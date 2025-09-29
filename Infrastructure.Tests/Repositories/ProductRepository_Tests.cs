using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Xunit;

namespace Infrastructure.Tests.Repositories;

public class ProductRepository_Tests
{
    [Fact]
    public void ProductRepository_AddProductToList_ShouldAddProduct()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product = new Product { Id = "1", Title = "Test Product", Price = 10.99m };

        // act
        productRepository.AddProductToList(product);

        // assert
        var response = productRepository.GetProductsFromList();
        Assert.Single(response);
        Assert.Equal(product, response.First());
    }

    [Fact]
    public void ProductRepository_AddProductToList_ShouldMaintainProductOrder()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product1 = new Product { Id = "1", Title = "First Product", Price = 5.99m };
        var product2 = new Product { Id = "2", Title = "Second Product", Price = 15.99m };
        var product3 = new Product { Id = "3", Title = "Third Product", Price = 25.99m };

        // act
        productRepository.AddProductToList(product1);
        productRepository.AddProductToList(product2);
        productRepository.AddProductToList(product3);
        var response = productRepository.GetProductsFromList().ToList();

        // assert
        Assert.Equal(3, response.Count);
        Assert.Equal(product1, response[0]);
        Assert.Equal(product2, response[1]);
        Assert.Equal(product3, response[2]);
    }

    [Fact]
    public void ProductRepository_GetProductsFromList_ShouldReturnEmptyListInitially()
    {
        // arrange
        var productRepository = new ProductRepository();

        // act
        var response = productRepository.GetProductsFromList();

        // assert
        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public void ProductRepository_GetProductsFromList_ShouldReturnAllAddedProducts()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product1 = new Product { Id = "1", Title = "Product 1", Price = 5.99m };
        var product2 = new Product { Id = "2", Title = "Product 2", Price = 15.99m };

        // act
        productRepository.AddProductToList(product1);
        productRepository.AddProductToList(product2);
        var response = productRepository.GetProductsFromList().ToList();

        // assert
        Assert.Equal(2, response.Count);
        Assert.Contains(product1, response);
        Assert.Contains(product2, response);
    }

    [Fact]
    public void ProductRepository_GetProductById_ShouldReturnCorrectProduct()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product1 = ProductFactory.Create("Product 1", 5.99m);
        var product2 = ProductFactory.Create("Product 2", 15.99m);
        productRepository.AddProductToList(product1);
        productRepository.AddProductToList(product2);

        // act
        var foundProduct = productRepository.GetProductById(product2.Id);

        // assert
        Assert.NotNull(foundProduct);
        Assert.Equal(product2.Id, foundProduct.Id);
        Assert.Equal("Product 2", foundProduct.Title);
        Assert.Equal(15.99m, foundProduct.Price);
    }

    [Fact]
    public void ProductRepository_GetProductById_ShouldReturnNullIfNotFound()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product = ProductFactory.Create("Product 1", 5.99m);
        productRepository.AddProductToList(product);

        // act
        var foundProduct = productRepository.GetProductById("999");

        // assert
        Assert.Null(foundProduct);
    }
}