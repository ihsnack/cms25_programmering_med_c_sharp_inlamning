using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IProductRepository
{
    void AddProductToList(Product product);
    Product GetProductById(string id);
    IEnumerable<Product> GetProductsFromList();
}
