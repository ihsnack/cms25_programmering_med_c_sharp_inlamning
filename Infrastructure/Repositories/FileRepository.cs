using Infrastructure.Interfaces;

namespace Infrastructure.Repositories
{
    public class FileRepository : IFileRepository
    {
        private string _directoryPath;
        private string _filePath;

        public FileRepository()
        {
            _directoryPath = Path.Combine(AppContext.BaseDirectory, "Data");
            _filePath = Path.Combine(_directoryPath, "products.json");
        }

        public async Task<string> GetContentFromFileAsync(CancellationToken cancellationToken)
        {
            if (File.Exists(_filePath))
            {
                return await File.ReadAllTextAsync(_filePath, cancellationToken);
            }

            return string.Empty;
        }

        public async Task SaveContentToFileAsync(string content, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            using var sw = new StreamWriter(_filePath);
            await sw.WriteAsync(content.AsMemory(), cancellationToken);
            await sw.FlushAsync(cancellationToken);
        }
    }
}