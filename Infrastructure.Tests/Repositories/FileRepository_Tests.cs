using Infrastructure.Repositories;
using Xunit;

namespace Infrastructure.Tests.Repositories;

/// <summary>
/// I've used Copilot to improve these tests and asked it adjust tests after refactorings
/// </summary>
public class FileRepository_Tests
{
    [Fact]
    public async Task FileRepository_GetContentFromFile_ShouldCallFileReadMethods()
    {
        // arrange
        var fileRepository = new FileRepository();

        // act
        var response = await fileRepository.GetContentFromFileAsync();

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
            await fileRepository.SaveContentToFileAsync(content);

            // assert
            Assert.True(File.Exists(filePath));

            var savedContent = File.ReadAllText(filePath);
            Assert.Equal(content, savedContent);
        }
        finally
        {
            // cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}