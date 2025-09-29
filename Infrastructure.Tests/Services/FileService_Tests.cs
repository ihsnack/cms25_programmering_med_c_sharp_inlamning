using System.Text.Json;
using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Services;

public class FileService_Tests
{
    private Category GetTestCategory() => new Category { Name = "Test Category" };
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
    public async Task FileService_SaveToFileAsync_ShouldHandleNullContent()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        fileRepositoryMock.Setup(fr => fr.SaveContentToFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // act
        var response = await fileService.SaveToFileAsync(null!);

        // assert
        fileRepositoryMock.Verify(fr => fr.SaveContentToFileAsync(It.IsAny<string>()), Times.Once);
        Assert.Equal("Saved list to file successfully.", response.Message);
        Assert.True(response.Success);
        Assert.True(response.Result);
    }

    [Fact]
    public async Task FileService_SaveToFileAsync_ShouldCallRepositoryOnce()
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
        fileRepositoryMock.Verify(fr => fr.SaveContentToFileAsync(It.IsAny<string>()), Times.Once);
        Assert.True(response.Success);
        Assert.True(response.Result);
        Assert.Equal("test product", testProduct.Title);
        Assert.Equal(9.99m, testProduct.Price);
        Assert.Equal(expectedId, testProduct.Id);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldRespondIfFileDoesntExist()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync(""));

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.Equal("No file to load content from.", response.Message);
        Assert.Null(response.Result);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldReturnContentIfFileExists()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var product1 = ProductFactory.Create("existing product", 5.99m, GetTestCategory(), GetTestManufacturer());
        var product2 = ProductFactory.Create("New File Product", 15.99m, GetTestCategory(), GetTestManufacturer());
        var expectedId1 = product1.Id;
        var expectedId2 = product2.Id;

        var productsInFile = new List<Product> { product1, product2 };
        var content = JsonSerializer.Serialize(productsInFile);
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync(content);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.Equal("Loaded list successfully from file.", response.Message);
        Assert.NotNull(response.Result);
        Assert.Equal(2, response.Result.Count());
        Assert.Equal("existing product", response.Result.First().Title);
        Assert.Equal(5.99m, response.Result.First().Price);
        Assert.Equal(expectedId1, response.Result.First().Id);
        Assert.Equal("New File Product", response.Result.Last().Title);
        Assert.Equal(15.99m, response.Result.Last().Price);
        Assert.Equal(expectedId2, response.Result.Last().Id);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldCatchIfErrorIsThrown()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ThrowsAsync(new Exception("Unknown Error."));

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.Equal("Error when trying to read file: Unknown Error.", response.Message);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldHandleNullContent()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync((string)null!);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        Assert.Equal("No file to load content from.", response.Message);
        Assert.Null(response.Result);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task FileService_LoadFromFileAsync_ShouldCallRepositoryOnce()
    {
        // arrange
        var fileRepositoryMock = new Mock<IFileRepository>();
        var fileRepository = fileRepositoryMock.Object;
        var fileService = new FileService(fileRepository);

        var testProduct = ProductFactory.Create("test product", 9.99m, GetTestCategory(), GetTestManufacturer());
        var expectedId = testProduct.Id;

        var productsInFile = new List<Product> { testProduct };
        var content = JsonSerializer.Serialize(productsInFile);
        fileRepositoryMock.Setup(fr => fr.GetContentFromFileAsync()).ReturnsAsync(content);

        // act
        var response = await fileService.LoadFromFileAsync();

        // assert
        fileRepositoryMock.Verify(fr => fr.GetContentFromFileAsync(), Times.Once);
        Assert.NotNull(response.Result);
        Assert.Single(response.Result);
        Assert.Equal("test product", response.Result.First().Title);
        Assert.Equal(9.99m, response.Result.First().Price);
        Assert.Equal(expectedId, response.Result.First().Id);
    }
}