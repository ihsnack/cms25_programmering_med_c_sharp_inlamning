using System.Text.Json;
using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace Infrastructure.Services;

public class FileService : IFileService
{
    private IFileRepository _fileRepository;

    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<ResponseResult<bool>> SaveToFileAsync(IEnumerable<Product> productList)
    {
        try
        {
            var json = JsonSerializer.Serialize(productList, new JsonSerializerOptions { WriteIndented = true });
            await _fileRepository.SaveContentToFileAsync(json);

            return new ResponseResult<bool>
            {
                Message = "Saved list to file successfully.",
                Success = true,
                Result = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = $"Error when trying to save file: {ex.Message}",
                Result = false
            };
        }
    }

    public async Task<ResponseResult<IEnumerable<Product>>> LoadFromFileAsync()
    {
        try
        {
            var content = await _fileRepository.GetContentFromFileAsync();

            if (string.IsNullOrEmpty(content))
            {
                return new ResponseResult<IEnumerable<Product>>
                {
                    Message = "No file to load content from.",
                    Success = true
                };
            }

            var fileProducts = JsonSerializer.Deserialize<List<Product>>(content) ?? [];

            return new ResponseResult<IEnumerable<Product>>
            {
                Message = "Loaded list successfully from file.",
                Success = true,
                Result = fileProducts
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<IEnumerable<Product>>
            {
                Message = $"Error when trying to read file: {ex.Message}",
                Success = false
            };
        }
    }
}