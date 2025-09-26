using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    ResponseResult<IEnumerable<Product>> GetProducts();
    Task<ResponseResult<IEnumerable<Product>>> LoadProductsAsync();
    Task<ResponseResult<bool>> SaveProductsAsync();
    ResponseResult<bool> CreateProduct(Product product);
}

