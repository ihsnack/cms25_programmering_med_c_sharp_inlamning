using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    public interface IFileService
    {
        Task<ResponseResult<IEnumerable<Product>>> LoadFromFileAsync();
        Task<ResponseResult<bool>> SaveToFileAsync(IEnumerable<Product> content);
    }
}