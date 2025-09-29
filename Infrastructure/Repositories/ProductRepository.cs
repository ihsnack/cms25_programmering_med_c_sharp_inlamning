using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = [];

    public void AddProductToList(Product product)
    {
        _products.Add(product);
    }

    public IEnumerable<Product> GetProductsFromList()
    {
        return _products;
    }

    public Product GetProductById(string id)
    {
        return _products.FirstOrDefault(e => e.Id == id)!;
    }
}
