using System.Text.Json;
using System.Xml.Linq;
using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Services;

/// <summary>
/// I've used Copilot to improve these tests and asked it adjust tests after refactorings
/// </summary>
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

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
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

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
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

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
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
    public async Task ProductService_LoadProducts_ShouldHandleFileServiceFailure()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = false,
            Message = "File read error"
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("File read error", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectProductsWithEmptyTitle()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("", 10.99m, GetTestCategory(), GetTestManufacturer());

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = new List<Product> { invalidProduct }
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("File contains product with empty or null title. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectProductsWithNegativePrice()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("Valid Title", -5.99m, GetTestCategory(), GetTestManufacturer());

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = new List<Product> { invalidProduct }
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product has an invalid price. Price must be greater than zero. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectProductsWithZeroPrice()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("Valid Title", 0m, GetTestCategory(), GetTestManufacturer());

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = new List<Product> { invalidProduct }
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product has an invalid price. Price must be greater than zero. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectProductsWithEmptyCategoryName()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("Valid Title", 10.99m, new Category { Name = "" }, GetTestManufacturer());

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = new List<Product> { invalidProduct }
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product category name needs to be provided. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectProductsWithEmptyManufacturerName()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("Valid Title", 10.99m, GetTestCategory(), new Manufacturer { Name = "" });

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = new List<Product> { invalidProduct }
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product manufacturer name needs to be provided. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectProductsWithNullCategory()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("Test Product", 9.99m, null!, GetTestManufacturer());

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = new List<Product> { invalidProduct }
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product category name needs to be provided. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldRejectProductsWithNullManufacturer()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var invalidProduct = ProductFactory.Create("Test Product", 9.99m, GetTestCategory(), null!);

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<IEnumerable<Product>>
        {
            Success = true,
            Result = new List<Product> { invalidProduct }
        });

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product manufacturer name needs to be provided. File load aborted.", response.Message);
    }

    [Fact]
    public async Task ProductService_LoadProducts_ShouldHandleException()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        fileServiceMock.Setup(fs => fs.LoadFromFileAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // act
        var response = await productService.LoadProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Test exception", response.Message);
    }

    [Fact]
    public async Task ProductService_SaveProducts_ShouldReturnSuccessWhenFileServiceSucceeds()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var products = new List<Product> { ProductFactory.Create("Test Product", 9.99m, GetTestCategory(), GetTestManufacturer()) };

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(products);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = true,
            Message = "Saved successfully"
        });

        // act
        var response = await productService.SaveProductsAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("Products saved to file successfully.", response.Message);
        Assert.True(response.Result);
    }

    [Fact]
    public async Task ProductService_SaveProducts_ShouldReturnFailureWhenFileServiceFails()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var products = new List<Product> { ProductFactory.Create("Test Product", 9.99m, GetTestCategory(), GetTestManufacturer()) };

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(products);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = false,
            Message = "File save error"
        });

        // act
        var response = await productService.SaveProductsAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("File save error", response.Message);
    }

    [Fact]
    public async Task ProductService_SaveProducts_ShouldHandleException()
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
    public void ProductService_GetProducts_ShouldReturnEmptyListMessage()
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
        Assert.NotNull(response.Result);
        Assert.Empty(response.Result);
    }

    [Fact]
    public void ProductService_GetProducts_ShouldReturnProductsList()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var products = new List<Product> { ProductFactory.Create("Test Product", 9.99m, GetTestCategory(), GetTestManufacturer()) };
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(products);

        // act
        var response = productService.GetProducts();

        // assert
        Assert.True(response.Success);
        Assert.Equal("Products retrieved successfully.", response.Message);
        Assert.NotNull(response.Result);
        Assert.Single(response.Result);
    }

    [Fact]
    public void ProductService_GetProducts_ShouldHandleException()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Throws(new Exception("Repository error"));

        // act
        var response = productService.GetProducts();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Repository error", response.Message);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectNullProduct()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        // act
        var response = await productService.CreateProductAsync(null!);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product cannot be null.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectEmptyTitle()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);


        var product = ProductFactory.Create("", 9.99m, GetTestCategory(), GetTestManufacturer());

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product title cannot be empty or whitespace.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectNegativePrice()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);


        var product = ProductFactory.Create("Valid Title", -5.99m, GetTestCategory(), GetTestManufacturer());

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product price must be greater than zero.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectZeroPrice()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);


        var product = ProductFactory.Create("Valid Title", 0m, GetTestCategory(), GetTestManufacturer());

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product price must be greater than zero.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectEmptyCategoryName()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);


        var product = ProductFactory.Create("Valid Title", 9.99m, new Category { Name = "" }, GetTestManufacturer());

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product category name needs to be provided", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectEmptyManufacturerName()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 9.99m, GetTestCategory(), new Manufacturer { Name = "" });

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product manufacturer name needs to be provided", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectNullCategory()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);


        var product = ProductFactory.Create("Valid Title", 9.99m, null!, GetTestManufacturer());

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product category name needs to be provided", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectNullManufacturer()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);


        var product = ProductFactory.Create("Valid Title", 9.99m, GetTestCategory(), null!);

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product manufacturer name needs to be provided", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldRejectDuplicateProductName()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var existingProduct = ProductFactory.Create("Existing Product", 15.99m, GetTestCategory(), GetTestManufacturer());
        var newProduct = ProductFactory.Create("EXISTING PRODUCT", 9.99m, GetTestCategory(), GetTestManufacturer()); // Different case

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product> { existingProduct });

        // act
        var response = await productService.CreateProductAsync(newProduct);

        // assert
        Assert.False(response.Success);
        Assert.Equal("A product with the same name already exists.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldCreateProductSuccessfully()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("New Product", 9.99m, GetTestCategory(), GetTestManufacturer());

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = true,
            Message = "Saved successfully"
        });

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.True(response.Success);
        Assert.Equal("Product was created successfully.", response.Message);
        Assert.True(response.Result);
        productRepositoryMock.Verify(pr => pr.AddProductToList(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldHandleSaveFailure()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("New Product", 9.99m, GetTestCategory(), GetTestManufacturer());

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = false,
            Message = "Save failed"
        });

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Save failed", response.Message);
        productRepositoryMock.Verify(pr => pr.AddProductToList(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task ProductService_CreateProduct_ShouldHandleException()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("New Product", 9.99m, GetTestCategory(), GetTestManufacturer());

        // Setup to return empty list first (for validation), then throw on AddProductToList
        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        productRepositoryMock.Setup(pr => pr.AddProductToList(It.IsAny<Product>())).Throws(new Exception("Repository error"));

        // act
        var response = await productService.CreateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Repository error", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectNullProduct()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        // act
        var response = await productService.UpdateProductAsync(null!);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product cannot be null.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectEmptyTitle()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("", 9.99m, GetTestCategory(), GetTestManufacturer());

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product title cannot be empty or whitespace.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectNegativePrice()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", -5.99m, GetTestCategory(), GetTestManufacturer());

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product price must be greater than zero.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectZeroPrice()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 0m, GetTestCategory(), GetTestManufacturer());

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product price must be greater than zero.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectNullProductId()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 9.99m, GetTestCategory(), GetTestManufacturer());
        product.Id = null!; // Set ID to null

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product ID cannot be null or empty.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectEmptyProductId()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 9.99m, GetTestCategory(), GetTestManufacturer());
        product.Id = "";

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product ID cannot be null or empty.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectWhitespaceProductId()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 9.99m, GetTestCategory(), GetTestManufacturer());
        product.Id = "   ";

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product ID cannot be null or empty.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectNullCategory()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 9.99m, null!, GetTestManufacturer());

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product category name needs to be provided", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectNullManufacturer()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 9.99m, GetTestCategory(), null!);

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product manufacturer name needs to be provided", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldRejectDuplicateProductName()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var existingProduct = ProductFactory.Create("Existing Product", 15.99m, GetTestCategory(), GetTestManufacturer());
        var anotherProduct = ProductFactory.Create("Another Product", 25.99m, GetTestCategory(), GetTestManufacturer());
        var updateProduct = ProductFactory.Create("EXISTING PRODUCT", 19.99m, GetTestCategory(), GetTestManufacturer());
        updateProduct.Id = anotherProduct.Id; // Set the ID to match anotherProduct for update

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product> { existingProduct, anotherProduct });
        productRepositoryMock.Setup(pr => pr.GetProductByIdFromList(anotherProduct.Id)).Returns(anotherProduct);

        // act
        var response = await productService.UpdateProductAsync(updateProduct);

        // assert
        Assert.False(response.Success);
        Assert.Equal("A product with the same name already exists.", response.Message);
        Assert.False(response.Result);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldAllowKeepingSameTitle()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var existingProduct = ProductFactory.Create("Existing Product", 15.99m, GetTestCategory(), GetTestManufacturer());
        var updateProduct = ProductFactory.Create("EXISTING PRODUCT", 19.99m, new Category { Name = "Updated Category" }, new Manufacturer { Name = "Updated Manufacturer" });
        updateProduct.Id = existingProduct.Id;

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product> { existingProduct });
        productRepositoryMock.Setup(pr => pr.GetProductByIdFromList(existingProduct.Id)).Returns(existingProduct);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = true,
            Message = "Saved successfully"
        });

        // act
        var response = await productService.UpdateProductAsync(updateProduct);

        // assert
        Assert.True(response.Success);
        Assert.Equal("Product was updated.", response.Message);
        Assert.True(response.Result);

        // Verify that the existing product's properties were actually updated
        Assert.Equal("EXISTING PRODUCT", existingProduct.Title);
        Assert.Equal(19.99m, existingProduct.Price);
        Assert.Equal("Updated Category", existingProduct.Category.Name);
        Assert.Equal("Updated Manufacturer", existingProduct.Manufacturer.Name);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldHandleProductNotFound()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 9.99m, GetTestCategory(), GetTestManufacturer());
        product.Id = "non-existent-id";

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product>());
        productRepositoryMock.Setup(pr => pr.GetProductByIdFromList("non-existent-id")).Returns((Product)null!);

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Could not find product to update.", response.Message);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldUpdateProductSuccessfully()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var existingProduct = ProductFactory.Create("Existing Product", 15.99m, GetTestCategory(), GetTestManufacturer());
        var updateProduct = ProductFactory.Create("Updated Title", 19.99m, new Category { Name = "Updated Category" }, new Manufacturer { Name = "Updated Manufacturer" });
        updateProduct.Id = existingProduct.Id;

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product> { existingProduct });
        productRepositoryMock.Setup(pr => pr.GetProductByIdFromList(existingProduct.Id)).Returns(existingProduct);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = true,
            Message = "Saved successfully"
        });

        // act
        var response = await productService.UpdateProductAsync(updateProduct);

        // assert
        Assert.True(response.Success);
        Assert.Equal("Product was updated.", response.Message);
        Assert.True(response.Result);

        // Verify that the existing product's properties were actually updated
        Assert.Equal("Updated Title", existingProduct.Title);
        Assert.Equal(19.99m, existingProduct.Price);
        Assert.Equal("Updated Category", existingProduct.Category.Name);
        Assert.Equal("Updated Manufacturer", existingProduct.Manufacturer.Name);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldHandleSaveFailure()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var existingProduct = ProductFactory.Create("Existing Product", 15.99m, GetTestCategory(), GetTestManufacturer());
        var updateProduct = ProductFactory.Create("Updated Title", 19.99m, GetTestCategory(), GetTestManufacturer());
        updateProduct.Id = existingProduct.Id;

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Returns(new List<Product> { existingProduct });
        productRepositoryMock.Setup(pr => pr.GetProductByIdFromList(existingProduct.Id)).Returns(existingProduct);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = false,
            Message = "Save failed"
        });

        // act
        var response = await productService.UpdateProductAsync(updateProduct);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Save failed", response.Message);
    }

    [Fact]
    public async Task ProductService_UpdateProduct_ShouldHandleException()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var product = ProductFactory.Create("Valid Title", 9.99m, GetTestCategory(), GetTestManufacturer());

        productRepositoryMock.Setup(pr => pr.GetProductsFromList()).Throws(new Exception("Repository error"));

        // act
        var response = await productService.UpdateProductAsync(product);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Repository error", response.Message);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldRejectNullId()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        // act
        var response = await productService.RemoveProductAsync(null!);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product ID cannot be null or empty.", response.Message);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldRejectEmptyId()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        // act
        var response = await productService.RemoveProductAsync("");

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product ID cannot be null or empty.", response.Message);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldRejectWhitespaceId()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        // act
        var response = await productService.RemoveProductAsync("   ");

        // assert
        Assert.False(response.Success);
        Assert.Equal("Product ID cannot be null or empty.", response.Message);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldRemoveProductSuccessfully()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var productId = "test-id";

        productRepositoryMock.Setup(pr => pr.RemoveProductFromList(productId)).Returns(1);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = true,
            Message = "Saved successfully"
        });

        // act
        var response = await productService.RemoveProductAsync(productId);

        // assert
        Assert.True(response.Success);
        Assert.Equal("Removed product successfully.", response.Message);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldHandleProductNotFound()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var productId = "non-existent-id";

        productRepositoryMock.Setup(pr => pr.RemoveProductFromList(productId)).Returns(0);

        // act
        var response = await productService.RemoveProductAsync(productId);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Could not find product to remove.", response.Message);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldHandleSaveFailure()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var productId = "test-id";

        productRepositoryMock.Setup(pr => pr.RemoveProductFromList(productId)).Returns(1);
        fileServiceMock.Setup(fs => fs.SaveToFileAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseResult<bool>
        {
            Success = false,
            Message = "Save failed"
        });

        // act
        var response = await productService.RemoveProductAsync(productId);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Save failed", response.Message);
    }

    [Fact]
    public async Task ProductService_RemoveProduct_ShouldHandleException()
    {
        // arrange
        var fileServiceMock = new Mock<IFileService>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var productService = new ProductService(productRepositoryMock.Object, fileServiceMock.Object);

        var productId = "test-id";

        productRepositoryMock.Setup(pr => pr.RemoveProductFromList(productId)).Throws(new Exception("Repository error"));

        // act
        var response = await productService.RemoveProductAsync(productId);

        // assert
        Assert.False(response.Success);
        Assert.Equal("Could not perform removal of product.", response.Message);
    }
}
