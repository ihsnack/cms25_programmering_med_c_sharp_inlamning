namespace Infrastructure.Interfaces
{
    public interface IProduct
    {
        string Id { get; set; }
        decimal Price { get; set; }
        string Title { get; set; }
    }
}