namespace Infrastructure.Interfaces;

public interface IFileRepository
{
    Task<string> GetContentFromFileAsync();
    Task SaveContentToFileAsync(string content);
}
