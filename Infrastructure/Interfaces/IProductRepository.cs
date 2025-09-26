using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IProductRepository
{
    void AddProductToList(Product product);
    IEnumerable<Product> GetProductsFromList();
}
