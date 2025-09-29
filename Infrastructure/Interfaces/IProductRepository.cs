using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IProductRepository
{
    void AddProductToList(Product product);
    Product GetProductByIdFromList(string id);
    IEnumerable<Product> GetProductsFromList();
    int RemoveProductFromList(string id);
}
