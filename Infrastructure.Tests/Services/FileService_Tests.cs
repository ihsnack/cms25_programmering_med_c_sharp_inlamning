using System.Text.Json;
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
public class FileService_Tests
{
    private Category GetTestCategory() => new Category { Name = "Clothes" };
    private Manufacturer GetTestManufacturer() => new Manufacturer { Name = "Test Manufacturer" };

    [Fact]
    public async Task FileService_SaveToFileAsync_ShouldReturnMessageIfSuccess()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var testProduct = ProductFactory.Create("test product", 9.99m, GetTestCategory(), GetTestManufacturer());
        var expectedId = testProduct.Id;
        var products = new List<Product> { testProduct };

        fileRepositoryMock.Setup(fr => fr.SaveContentToFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // act
        var response = await fileService.SaveToFileAsync(products);

        // assert
        Assert.Equal("Saved list to file successfully.", response.Message);
        Assert.True(response.Success);
        Assert.True(response.Result);
        Assert.Equal("test product", testProduct.Title);
        Assert.Equal(9.99m, testProduct.Price);
        Assert.Equal(expectedId, testProduct.Id);
    }

    [Fact]
    public async Task FileService_SaveToFileAsync_ShouldCatchIfErrorIsThrow()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var testProduct = ProductFactory.Create("test product", 9.99m, GetTestCategory(), GetTestManufacturer());
        var expectedId = testProduct.Id;
        var products = new List<Product> { testProduct };

        fileRepositoryMock.Setup(fr => fr.SaveContentToFileAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Unknown Error."));

        // act
        var response = await fileService.SaveToFileAsync(products);

        // assert
        Assert.Equal("Error when trying to save file: Unknown Error.", response.Message);
        Assert.False(response.Success);
        Assert.False(response.Result);
        Assert.Equal("test product", testProduct.Title);
        Assert.Equal(9.99m, testProduct.Price);
        Assert.Equal(expectedId, testProduct.Id);
    }

    [Fact]
    public async Task FileService_SaveToFileAsync_ShouldHandleEmptyProductList()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var products = new List<Product>();

        fileRepositoryMock.Setup(fr => fr.SaveContentToFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // act
        var response = await fileService.SaveToFileAsync(products);

        // assert
        Assert.Equal("Saved list to file successfully.", response.Message);
        Assert.True(response.Success);
        Assert.True(response.Result);
    }

    [Fact]
    public async Task FileService_SaveToFileAsync_ShouldSerializeMultipleProducts()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var products = new List<Product>
        {
            ProductFactory.Create("Product 1", 9.99m, GetTestCategory(), GetTestManufacturer()),
            ProductFactory.Create("Product 2", 19.99m, GetTestCategory(), GetTestManufacturer()),
            ProductFactory.Create("Product 3", 29.99m, GetTestCategory(), GetTestManufacturer())
        };

        string capturedJson = null!;
        fileRepositoryMock.Setup(fr => fr.SaveContentToFileAsync(It.IsAny<string>()))
            .Callback<string>(json => capturedJson = json)
            .Returns(Task.CompletedTask);

        // act
        var response = await fileService.SaveToFileAsync(products);

        // assert
        Assert.True(response.Success);
        Assert.NotNull(capturedJson);

        var deserializedProducts = JsonSerializer.Deserialize<List<Product>>(capturedJson);
        Assert.Equal(3, deserializedProducts!.Count);
        Assert.Equal("Product 1", deserializedProducts[0].Title);
        Assert.Equal("Product 2", deserializedProducts[1].Title);
        Assert.Equal("Product 3", deserializedProducts[2].Title);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldReturnEmptyResultForEmptyFile()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync(string.Empty);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("No file to load content from.", response.Message);
        Assert.Null(response.Result);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldReturnEmptyResultForNullContent()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync((string)null!);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("No file to load content from.", response.Message);
        Assert.Null(response.Result);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldDeserializeValidJson()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var products = new List<Product>
        {
            ProductFactory.Create("Test Product 1", 9.99m, GetTestCategory(), GetTestManufacturer()),
            ProductFactory.Create("Test Product 2", 19.99m, GetTestCategory(), GetTestManufacturer())
        };

        var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync(json);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("Loaded list successfully from file.", response.Message);
        Assert.NotNull(response.Result);
        Assert.Equal(2, response.Result.Count());

        var resultProducts = response.Result.ToList();
        Assert.Equal("Test Product 1", resultProducts[0].Title);
        Assert.Equal("Test Product 2", resultProducts[1].Title);
        Assert.Equal(9.99m, resultProducts[0].Price);
        Assert.Equal(19.99m, resultProducts[1].Price);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldReturnEmptyListForEmptyJsonArray()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var json = "[]";
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync(json);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("Loaded list successfully from file.", response.Message);
        Assert.NotNull(response.Result);
        Assert.Empty(response.Result);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldHandleInvalidJson()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var invalidJson = "{ invalid json content }";
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync(invalidJson);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.False(response.Success);
        Assert.StartsWith("Error when trying to read file:", response.Message);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldHandleFileRepositoryException()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ThrowsAsync(new Exception("File access error"));

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.False(response.Success);
        Assert.Equal("Error when trying to read file: File access error", response.Message);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldHandleNullJsonDeserialization()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var json = "null";
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync(json);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.True(response.Success);
        Assert.Equal("Loaded list successfully from file.", response.Message);
        Assert.NotNull(response.Result);
        Assert.Empty(response.Result);
    }
}