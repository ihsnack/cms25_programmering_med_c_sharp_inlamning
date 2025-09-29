using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class ProductDetailsViewModel(IServiceProvider serviceProvider) : ObservableObject
{

    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [ObservableProperty]
    public Product _product = new()
    {
        Manufacturer = new Manufacturer(),
        Category = new Category()
    };

    [RelayCommand]
    public void NavigateToList()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ListProductViewModel>();
    }

    [RelayCommand]
    public void NavigateToProductEdit()
    {
        var productEditView = _serviceProvider.GetRequiredService<EditProductViewModel>();
        productEditView.Product = Product;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<EditProductViewModel>();
    }
}
