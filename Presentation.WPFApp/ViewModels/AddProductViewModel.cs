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
    public Product _product = new Product();

    [RelayCommand]
    public void NavigateToList()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ListProductViewModel>();
    }

    [RelayCommand]
    public void SaveProduct()
    {
        // Provide default Category and Manufacturer for now
        var category = Product.Category ?? new Category { Name = "Default Category" };
        var manufacturer = Product.Manufacturer ?? new Manufacturer { Name = "Default Manufacturer" };
        var productInstance = ProductFactory.Create(Product.Title, Product.Price, category, manufacturer);

        var response = _productService.CreateProduct(productInstance);

        NavigateToList();
    }

}
