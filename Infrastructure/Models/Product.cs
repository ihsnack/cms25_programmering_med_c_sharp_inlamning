using Infrastructure.Interfaces;

namespace Infrastructure.Models;

public enum Category
{
    Clothes,
    Technology
}

public class Product : IProduct
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public Category Category { get; set; }
    public Manufacturer Manufacturer { get; set; } = null!;
}
