using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class AddProductViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
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
    public void SaveProduct()
    {
        var category = new Category { Name = Product.Manufacturer.Name };
        var manufacturer = new Manufacturer { Name = Product.Category.Name };
        var productInstance = ProductFactory.Create(Product.Title, Product.Price, category, manufacturer);

        var response = _productService.CreateProduct(productInstance);

        NavigateToList();
    }

}
