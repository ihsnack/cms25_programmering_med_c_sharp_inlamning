using Infrastructure.Interfaces;

namespace Infrastructure.Models;

public class Product : IProduct
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public Category Category { get; set; } = new Category { Name = null! };
    public Manufacturer Manufacturer { get; set; } = new Manufacturer { Name = null! };
}
