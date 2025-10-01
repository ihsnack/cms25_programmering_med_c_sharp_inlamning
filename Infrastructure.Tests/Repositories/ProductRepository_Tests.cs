using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Xunit;

namespace Infrastructure.Tests.Repositories;

/// <summary>
/// I've used Copilot to improve these tests and asked it adjust tests after refactorings
/// </summary>
public class ProductRepository_Tests
{
    private Category GetTestCategory() => new Category { Name = "Clothes" };
    private Manufacturer GetTestManufacturer() => new Manufacturer { Name = "Test Manufacturer" };

    [Fact]
    public void ProductRepository_AddProductToList_ShouldAddProduct()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product = ProductFactory.Create("Test Product", 10.99m, GetTestCategory(), GetTestManufacturer());

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
        var product1 = ProductFactory.Create("First Product", 5.99m, GetTestCategory(), GetTestManufacturer());
        var product2 = ProductFactory.Create("Second Product", 15.99m, GetTestCategory(), GetTestManufacturer());
        var product3 = ProductFactory.Create("Third Product", 25.99m, GetTestCategory(), GetTestManufacturer());

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
        var product1 = ProductFactory.Create("Product 1", 5.99m, GetTestCategory(), GetTestManufacturer());
        var product2 = ProductFactory.Create("Product 2", 15.99m, GetTestCategory(), GetTestManufacturer());

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
    public void ProductRepository_SetProductsToList_ShouldReplaceAllProducts()
    {
        // arrange
        var productRepository = new ProductRepository();
        var initialProduct = ProductFactory.Create("Initial Product", 5.99m, GetTestCategory(), GetTestManufacturer());
        productRepository.AddProductToList(initialProduct);

        var newProducts = new List<Product>
        {
            ProductFactory.Create("New Product 1", 10.99m, GetTestCategory(), GetTestManufacturer()),
            ProductFactory.Create("New Product 2", 15.99m, GetTestCategory(), GetTestManufacturer())
        };

        // act
        productRepository.SetProductsToList(newProducts);
        var response = productRepository.GetProductsFromList().ToList();

        // assert
        Assert.Equal(2, response.Count);
        Assert.DoesNotContain(initialProduct, response);
        Assert.Contains(newProducts[0], response);
        Assert.Contains(newProducts[1], response);
    }

    [Fact]
    public void ProductRepository_SetProductsToList_ShouldHandleEmptyList()
    {
        // arrange
        var productRepository = new ProductRepository();
        var initialProduct = ProductFactory.Create("Initial Product", 5.99m, GetTestCategory(), GetTestManufacturer());
        productRepository.AddProductToList(initialProduct);

        // act
        productRepository.SetProductsToList(new List<Product>());
        var response = productRepository.GetProductsFromList();

        // assert
        Assert.Empty(response);
    }

    [Fact]
    public void ProductRepository_GetProductByIdFromList_ShouldReturnCorrectProduct()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product1 = ProductFactory.Create("Product 1", 5.99m, GetTestCategory(), GetTestManufacturer());
        var product2 = ProductFactory.Create("Product 2", 15.99m, GetTestCategory(), GetTestManufacturer());

        productRepository.AddProductToList(product1);
        productRepository.AddProductToList(product2);

        // act
        var result = productRepository.GetProductByIdFromList(product1.Id);

        // assert
        Assert.NotNull(result);
        Assert.Equal(product1.Id, result.Id);
        Assert.Equal(product1.Title, result.Title);
    }

    [Fact]
    public void ProductRepository_GetProductByIdFromList_ShouldReturnNullForNonExistentId()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product = ProductFactory.Create("Test Product", 10.99m, GetTestCategory(), GetTestManufacturer());
        productRepository.AddProductToList(product);

        // act
        var result = productRepository.GetProductByIdFromList("non-existent-id");

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void ProductRepository_RemoveProductFromList_ShouldRemoveExistingProduct()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product1 = ProductFactory.Create("Product 1", 5.99m, GetTestCategory(), GetTestManufacturer());
        var product2 = ProductFactory.Create("Product 2", 15.99m, GetTestCategory(), GetTestManufacturer());

        productRepository.AddProductToList(product1);
        productRepository.AddProductToList(product2);

        // act
        var removedCount = productRepository.RemoveProductFromList(product1.Id);
        var remainingProducts = productRepository.GetProductsFromList().ToList();

        // assert
        Assert.Equal(1, removedCount);
        Assert.Single(remainingProducts);
        Assert.Equal(product2.Id, remainingProducts[0].Id);
    }

    [Fact]
    public void ProductRepository_RemoveProductFromList_ShouldReturnZeroForNonExistentProduct()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product = ProductFactory.Create("Test Product", 10.99m, GetTestCategory(), GetTestManufacturer());
        productRepository.AddProductToList(product);

        // act
        var removedCount = productRepository.RemoveProductFromList("non-existent-id");
        var remainingProducts = productRepository.GetProductsFromList().ToList();

        // assert
        Assert.Equal(0, removedCount);
        Assert.Single(remainingProducts);
        Assert.Equal(product.Id, remainingProducts[0].Id);
    }

    [Fact]
    public void ProductRepository_RemoveProductFromList_ShouldRemoveAllMatchingProducts()
    {
        // arrange
        var productRepository = new ProductRepository();
        var product1 = ProductFactory.Create("Product 1", 5.99m, GetTestCategory(), GetTestManufacturer());
        var product2 = ProductFactory.Create("Product 2", 15.99m, GetTestCategory(), GetTestManufacturer());
        var product3 = ProductFactory.Create("Product 3", 25.99m, GetTestCategory(), GetTestManufacturer());

        // Manually create duplicate ID scenario (shouldn't happen in normal use but tests the RemoveAll behavior)
        product3.Id = product1.Id;

        productRepository.AddProductToList(product1);
        productRepository.AddProductToList(product2);
        productRepository.AddProductToList(product3);

        // act
        var removedCount = productRepository.RemoveProductFromList(product1.Id);
        var remainingProducts = productRepository.GetProductsFromList().ToList();

        // assert
        Assert.Equal(2, removedCount);
        Assert.Single(remainingProducts);
        Assert.Equal(product2.Id, remainingProducts[0].Id);
    }
}