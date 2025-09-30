namespace Presentation.Interfaces
{
    public interface IMenuDialogs
    {
        Task AddProductDialogAsync();
        Task MenuOptionsDialogAsync();
        Task ViewLoadProductsFromFileAsync();
        Task ViewSaveListToFileAsync();
        void ViewProductsDialogAsync();
    }
}