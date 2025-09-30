using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class ProductAddViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;
    [ObservableProperty]
    public Product _product = new()
    {
        Manufacturer = new Manufacturer(),
        Category = new Category()
    };

    [ObservableProperty]
    public string _errorMessage = null!;

    [RelayCommand]
    public void NavigateToList()
    {
        ErrorMessage = null!;
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
    }

    [RelayCommand]
    public void SaveProduct()
    {

        ErrorMessage = null!;

        var category = new Category { Name = Product.Manufacturer.Name };
        var manufacturer = new Manufacturer { Name = Product.Category.Name };
        var productInstance = ProductFactory.Create(Product.Title, Product.Price, category, manufacturer);

        var response = _productService.CreateProduct(productInstance);

        if (!response.Success)
        {
            ErrorMessage = response.Message!;
            return;
        }


        NavigateToList();
    }

}
