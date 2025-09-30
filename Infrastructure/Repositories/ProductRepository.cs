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
    public void SetProductsToList(IEnumerable<Product> products)
    {
        _products.Clear();
        _products.AddRange(products);
    }

    public IEnumerable<Product> GetProductsFromList()
    {
        return _products;
    }

    public Product GetProductByIdFromList(string id)
    {
        return _products.FirstOrDefault(e => e.Id == id)!;
    }

    public int RemoveProductFromList(string id)
    {
        return _products.RemoveAll(e => e.Id == id);
    }
}
