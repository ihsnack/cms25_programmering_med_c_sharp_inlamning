using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    ResponseResult<IEnumerable<Product>> GetProducts();
    Task<ResponseResult<IEnumerable<Product>>> LoadProductsAsync();
    Task<ResponseResult<bool>> SaveProductsAsync();
    Task<ResponseResult<bool>> RemoveProductAsync(string id);
    Task<ResponseResult<bool>> UpdateProductAsync(Product product);
    Task<ResponseResult<bool>> CreateProductAsync(Product product);
    void Cancel();
}

