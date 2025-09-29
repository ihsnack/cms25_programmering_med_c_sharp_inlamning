using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class ProductDetailsViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
{

    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;

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

    [RelayCommand]
    public void Delete()
    {
        var result2 = _productService.GetProducts()?.Result;

        _productService.RemoveProduct(Product.Id);

        var result = _productService.GetProducts()?.Result;

        NavigateToList();

    }

}
