using Infrastructure.Interfaces;

namespace Infrastructure.Models;

public class Product : IProduct
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public Category Category { get; set; } = null!;
    public Manufacturer Manufacturer { get; set; } = null!;
}
