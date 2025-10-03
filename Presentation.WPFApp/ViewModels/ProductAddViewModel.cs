using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class ProductAddViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;

    [ObservableProperty]
    private Product _product = new()
    {
        Category = new Category(),
        Manufacturer = new Manufacturer(),
    };

    [ObservableProperty]
    private string _priceText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = null!;

    partial void OnPriceTextChanged(string value)
    {
        if (ProductValidationHelper.TryParsePrice(value, out decimal price))
        {
            _product.Price = price;
        }
        else if (string.IsNullOrWhiteSpace(value))
        {
            _product.Price = 0;
        }
        else
        {
            _product.Price = 0;
        }
    }

    [RelayCommand]
    public void NavigateToList()
    {
        ErrorMessage = null!;
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
    }

    [RelayCommand]
    public async Task SaveProduct()
    {
        try
        {
            ErrorMessage = null!;

            var productInstance = ProductFactory.Create(Product.Title, Product.Price, Product.Category, Product.Manufacturer);

            var response = await _productService.CreateProduct(productInstance);

            if (!response.Success)
            {
                ErrorMessage = response.Message!;
                return;
            }

            NavigateToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An unexpected error occurred: {ex.Message}";
        }
    }
}
