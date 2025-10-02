using Infrastructure.Repositories;
using Xunit;

namespace Infrastructure.Tests.Repositories;

/// <summary>
/// I've used Copilot to improve these tests and asked it adjust tests after refactorings
/// </summary>
public class FileRepository_Tests
{
    private void CleanupTestFile()
    {
        var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
        var filePath = Path.Combine(dataDir, "products.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task FileRepository_GetContentFromFile_ShouldCallFileReadMethods()
    {
        // arrange
        var fileRepository = new FileRepository();

        // act
        var response = await fileRepository.GetContentFromFileAsync(CancellationToken.None);

        // assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task FileRepository_SaveContentToFile_ShouldExecuteWithoutException()
    {
        // arrange
        var fileRepository = new FileRepository();
        var content = "test content";
        var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
        var filePath = Path.Combine(dataDir, "products.json");

        try
        {
            // act
            await fileRepository.SaveContentToFileAsync(content, CancellationToken.None);

            // assert
            Assert.True(File.Exists(filePath));

            var savedContent = File.ReadAllText(filePath);
            Assert.Equal(content, savedContent);
        }
        finally
        {
            // cleanup
            CleanupTestFile();
        }
    }

    [Fact]
    public async Task FileRepository_GetContentFromFile_ShouldReturnEmptyStringWhenFileDoesNotExist()
    {
        // arrange
        CleanupTestFile();
        var fileRepository = new FileRepository();

        // act
        var response = await fileRepository.GetContentFromFileAsync(CancellationToken.None);

        // assert
        Assert.Equal(string.Empty, response);
    }

    [Fact]
    public async Task FileRepository_SaveAndGetContent_ShouldPersistData()
    {
        // arrange
        var fileRepository = new FileRepository();
        var content = "test content for persistence";
        var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
        var filePath = Path.Combine(dataDir, "products.json");

        try
        {
            // act
            await fileRepository.SaveContentToFileAsync(content, CancellationToken.None);
            var retrievedContent = await fileRepository.GetContentFromFileAsync(CancellationToken.None);

            // assert
            Assert.Equal(content, retrievedContent);
        }
        finally
        {
            CleanupTestFile();
        }
    }
}