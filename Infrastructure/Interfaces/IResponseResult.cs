namespace Infrastructure.Interfaces
{
    public interface IResponseResult<T>
    {
        string? Message { get; set; }
        T? Result { get; set; }
        bool Success { get; set; }
    }
}