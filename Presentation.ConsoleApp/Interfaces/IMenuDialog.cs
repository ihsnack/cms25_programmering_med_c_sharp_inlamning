namespace Presentation.Interfaces
{
    public interface IMenuDialogs
    {
        void AddProductDialogAsync();
        Task MenuOptionsDialogAsync();
        Task ViewLoadProductsFromFileAsync();
        Task ViewSaveListToFileAsync();
        void ViewProductsDialogAsync();
    }
}