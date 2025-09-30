using System.Text.Json;
using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Services;

public class ProductService_Tests
{
    private Category GetTestCategory() => new Category { Name = "Clothes" };
    private Manufacturer GetTestManufacturer() => new Manufacturer { Name = "Test Manufacturer" };

    [Fact]
    public async Task ProductService_LoadProducts_ShouldHandleEmptyFile()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync()).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = null
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("File is empty or does not exist.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldLoadValidProducts()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var fileProduct = ProductFactory.Create("File Product", 15.99m, GetTestCategory(), GetTestManufacturer());
        var fileProducts = new List<Product> { fileProduct };

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync()).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = fileProducts
        });

        productRepositoryMock.SetupSequence(pr => pr.GetProductsFromList())
            .Returns(new List<Product>())
            .Returns(new List<Product> { fileProduct });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("Products loaded successfully from file.", response.Message);
        productRepositoryMock.Verify(pr => pr.SetProductsToList(It.Is<IEnumerable<Product>>(products => products.SequenceEqual(fileProducts))), Times.Once);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectNullProducts()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var fileProducts = new List<Product> { null! };

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync()).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = fileProducts
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("File contains null product entries. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectEmptyTitle()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("", 10.99m, GetTestCategory(), GetTestManufacturer());
        var fileProducts = new List<Product> { invalidProduct };

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync()).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = fileProducts
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("File contains product with empty or null title. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectNegativePrice()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("Invalid Product", -5.99m, GetTestCategory(), GetTestManufacturer());
        var fileProducts = new List<Product> { invalidProduct };

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync()).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = fileProducts
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product has an invalid negative price. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldHandleJsonException()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync()).Throws(new JsonException("Invalid JSON"));

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Invalid JSON", response.Message);
    }

    [Fact]
    public async Task ProductService_SaveProducts_ShouldReturnSuccess_WhenSaveSucceeds()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product1 = ProductFactory.Create("Product 1", 10.99m, GetTestCategory(), GetTestManufacturer());
        var product2 = ProductFactory.Create("Product 2", 20.99m, GetTestCategory(), GetTestManufacturer());
        var products = new List<Product> { product1, product2 };

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(products);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(products)).ReturnsAsync(new ResponseResult<bool>
        {
            Success = true,
            Message = "File saved successfully",
            Result = true
        });

        // act
        var response = await productService.SaveProductsAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("Products saved to file successfully.", response.Message);
        Assert.True(response.Result);
        productRepositoryMock.Verify(pr => pr.GetProductsFromList(), Times.Once);
        fileServiceMock.Verify(fs => fs.SaveToFileAsync(products), Times.Once);
    }

    [Fact]
    public async Task ProductService_SaveProducts_ShouldHandleEmptyProductList()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var emptyProducts = new List<Product>();

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(emptyProducts);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(emptyProducts)).ReturnsAsync(new ResponseResult<bool>
        {
            Success = true,
            Message = "Empty list saved",
            Result = true
        });

        // act
        var response = await productService.SaveProductsAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("Products saved to file successfully.", response.Message);
        Assert.True(response.Result);
        fileServiceMock.Verify(fs => fs.SaveToFileAsync(emptyProducts), Times.Once);
    }

    [Fact]
    public async Task ProductService_SaveProducts_ShouldCatchException()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Throws(new Exception("Repository error"));

        // act
        var response = await productService.SaveProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Repository error", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_SaveProducts_ShouldHandleFileServiceFailure()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var products = new List<Product> { ProductFactory.Create("Test", 10.99m, GetTestCategory(), GetTestManufacturer()) };

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(products);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(products)).Throws(new Exception("File system error"));

        // act
        var response = await productService.SaveProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("File system error", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_SaveProducts_ShouldHandleJsonException()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var products = new List<Product> { ProductFactory.Create("Test", 10.99m, GetTestCategory(), GetTestManufacturer()) };
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(products);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(products)).Throws(new Exception("Could not save."));

        // act
        var response = await productService.SaveProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Could not save.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldReturnFalse_WhenProductIsNull()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        // act
        var response = await productService.CreateProduct(null!);
        // assert
        Assert.False(response.Success);
        Assert.Equal("Product cannot be null.", response.Message);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldReturnFalse_WhenProductTitleIsNull()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create(null!, 10.99m, GetTestCategory(), GetTestManufacturer());
        // act
        var response = await productService.CreateProduct(product);
        // assert
        Assert.False(response.Success);
        Assert.Equal("Product title cannot be empty or whitespace.", response.Message);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldReturnFalse_WhenProductTitleIsEmpty()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create("", 10.99m, GetTestCategory(), GetTestManufacturer());
        // act
        var response = await productService.CreateProduct(product);
        // assert
        Assert.False(response.Success);
        Assert.Equal("Product title cannot be empty or whitespace.", response.Message);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldReturnFalse_WhenProductPriceIsNegative()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create("Jacket", -10.99m, GetTestCategory(), GetTestManufacturer());
        // act
        var response = await productService.CreateProduct(product);
        // assert
        Assert.False(response.Success);
        Assert.Equal("Product price cannot be negative.", response.Message);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldReturnTrue_WhenProductIsCreated()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create("Jacket", 10.99m, GetTestCategory(), GetTestManufacturer());
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        // act
        var response = await productService.CreateProduct(product);
        // assert
        Assert.True(response.Success);
        Assert.Equal("Product was created successfully.", response.Message);
        productRepositoryMock.Verify(pr => pr.GetProductsFromList(), Times.Exactly(2));
        productRepositoryMock.Verify(pr => pr.AddProductToList(It.Is<Product>(p => p.Title == "Jacket" && p.Price == 10.99m)), Times.Once);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldReturnFalse_WhenProductIsWithSameTitleExists()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var existingProduct = ProductFactory.Create("Jacket", 15.99m, GetTestCategory(), GetTestManufacturer());
        var existingProductsList = new List<Product> { existingProduct };
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(existingProductsList);
        var productDuplicate = ProductFactory.Create("Jacket", 10.99m, GetTestCategory(), GetTestManufacturer());
        // act
        var response = await productService.CreateProduct(productDuplicate);
        // assert
        Assert.False(response.Success);
        Assert.Equal("A product with the same name already exists.", response.Message);
        productRepositoryMock.Verify(pr => pr.GetProductsFromList(), Times.Once);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldCatchIfErrorIsThrow()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create("Jacket", 10.99m, GetTestCategory(), GetTestManufacturer());
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        productRepositoryMock.Setup(pr => pr.AddProductToList(It.IsAny<Product>())).Throws(new Exception("Unknown Error."));
        // act
        var response = await productService.CreateProduct(product);
        // assert
        Assert.False(response.Success);
        Assert.Equal("Unknown Error.", response.Message);
        productRepositoryMock.Verify(pr => pr.GetProductsFromList(), Times.Once);
        productRepositoryMock.Verify(pr => pr.AddProductToList(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldGenerateProductWithId()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create("Shoes", 49.99m, GetTestCategory(), GetTestManufacturer());
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        Product? addedProduct = null;
        productRepositoryMock.Setup(pr => pr.AddProductToList(It.IsAny<Product>()))
            .Callback<Product>(p => addedProduct = p);
        // act
        var response = await productService.CreateProduct(product);
        // assert
        Assert.True(response.Success);
        Assert.NotNull(addedProduct);
        Assert.False(string.IsNullOrWhiteSpace(addedProduct!.Id));
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldReturnFalse_WhenProductTitleIsCaseInsensitiveDuplicate()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var existingProduct = ProductFactory.Create("Jacket", 15.99m, GetTestCategory(), GetTestManufacturer());
        var existingProductsList = new List<Product> { existingProduct };
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(existingProductsList);
        var productDuplicate = ProductFactory.Create("jAcKeT", 10.99m, GetTestCategory(), GetTestManufacturer());
        // act
        var response = await productService.CreateProduct(productDuplicate);
        // assert
        Assert.False(response.Success);
        Assert.Equal("A product with the same name already exists.", response.Message);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldReturnFalse_WhenProductTitleIsWhitespaceOnly()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create("   ", 10.99m, GetTestCategory(), GetTestManufacturer());
        // act
        var response = await productService.CreateProduct(product);
        // assert
        Assert.False(response.Success);
        Assert.Equal("Product title cannot be empty or whitespace.", response.Message);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldGenerateProductWithGuidIdFormat()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create("Hat", 19.99m, GetTestCategory(), GetTestManufacturer());
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        Product? addedProduct = null;
        productRepositoryMock.Setup(pr => pr.AddProductToList(It.IsAny<Product>()))
            .Callback<Product>(p => addedProduct = p);
        // act
        var response = await productService.CreateProduct(product);
        // assert
        Assert.True(response.Success);
        Assert.NotNull(addedProduct);
        Assert.True(Guid.TryParse(addedProduct!.Id, out _));
    }

    [Fact]
    public void ProductService_GetProducts_ShouldReturnSuccess_WhenListIsEmpty()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        // act
        var response = productService.GetProducts();
        // assert
        Assert.True(response.Success);
        Assert.Equal("No products in list.", response.Message);
        Assert.Empty(response.Result!);
    }

    [Fact]
    public void ProductService_GetProducts_ShouldReturnSuccess_WhenListIsNotEmpty()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var products = new List<Product> { ProductFactory.Create("Test", 10.99m, GetTestCategory(), GetTestManufacturer()) };
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(products);
        // act
        var response = productService.GetProducts();
        // assert
        Assert.True(response.Success);
        Assert.Equal("Products retrieved successfully.", response.Message);
        Assert.Single(response.Result!);
        Assert.Equal("Test", response.Result!.First().Title);
    }

    [Fact]
    public void ProductService_GetProducts_ShouldReturnFailure_WhenExceptionThrown()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Throws(new Exception("Repo error"));
        // act
        var response = productService.GetProducts();
        // assert
        Assert.False(response.Success);
        Assert.Equal("Repo error", response.Message);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldReturnFalse_WhenProductNotFound()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var notFoundProduct = new Product { Id = "notfound", Title = "Test", Price = 1, Category = new Category { Name = "Test" }, Manufacturer = new Manufacturer { Name = "Test" } };
        productRepositoryMock.Setup(pr => pr.GetProductByIdFromList("notfound")).Returns((Product)null!);

        // act
        var response = await productService.UpdateProductAsync(notFoundProduct);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Could not find product to update.", response.Message);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldReturnTrue_WhenProductFound()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var product = ProductFactory.Create("Hat", 19.99m, GetTestCategory(), GetTestManufacturer());
        productRepositoryMock.Setup(pr => pr.GetProductByIdFromList(product.Id)).Returns(product);

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.True(response.Success);
        Assert.Equal("Product was updated.", response.Message);
        Assert.True(response.Result);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldRemoveProduct()
    {
        // arrange
        var productRepositoryMock = new Mock<IProductRepository>();
        var fileServiceMock = new Mock<IFileService>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var productId = "test-id";
        productRepositoryMock.Setup(pr => pr.RemoveProductFromList(productId)).Returns(1);

        // act
        var response = await productService.RemoveProduct(productId);

        // assert
        Assert.True(response.Success);
        Assert.Equal("Removed product successfully.", response.Message);
        productRepositoryMock.Verify(pr => pr.RemoveProductFromList(productId), Times.Once);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldReturnFalseIfNotFound()
    {
        // arrange
        var productRepositoryMock = new Mock<IProductRepository>();
        var fileServiceMock = new Mock<IFileService>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);
        var productId = "nonexistent-id";
        productRepositoryMock.Setup(pr => pr.RemoveProductFromList(productId)).Returns(0);

        // act
        var response = await productService.RemoveProduct(productId);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Could not find product to remove.", response.Message);
        productRepositoryMock.Verify(pr => pr.RemoveProductFromList(productId), Times.Once);
    }
}