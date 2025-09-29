using Infrastructure.Models;

namespace Infrastructure.Factories;

public static class ProductFactory
{
    public static Product Create(string title, decimal price, Category category, Manufacturer manufacturer)
    {
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Price = price,
            Category = category,
            Manufacturer = manufacturer
        };
    }
}
