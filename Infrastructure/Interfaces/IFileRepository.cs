namespace Infrastructure.Interfaces;

public interface IFileRepository
{
    Task<string> GetContentFromFileAsync(CancellationToken cancellationToken);
    Task SaveContentToFileAsync(string content, CancellationToken cancellationToken);
}
