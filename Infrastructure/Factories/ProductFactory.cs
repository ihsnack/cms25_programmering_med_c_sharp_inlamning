using Infrastructure.Models;

namespace Infrastructure.Factories;

public static class ProductFactory
{
    public static Product Create(string title, decimal price, Category? category, Manufacturer? manufacturer)
    {
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Price = price,
            Category = new Category { Name = category?.Name ?? string.Empty },
            Manufacturer = new Manufacturer { Name = manufacturer?.Name ?? string.Empty }
        };
    }
}
