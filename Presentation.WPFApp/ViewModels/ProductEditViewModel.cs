using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class ProductEditViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;

    [ObservableProperty]
    private Product _product = new();

    [ObservableProperty]
    private string _priceText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = null!;

    // I used Copilot to generate this method for not allowing non numerical values in the input
    partial void OnProductChanged(Product value)
    {
        if (value != null)
        {
            _priceText = value.Price.ToString(CultureInfo.InvariantCulture);
            OnPropertyChanged(nameof(PriceText));
        }
    }

    // I used Copilot to generate this method for not allowing non numerical values in the input
    partial void OnPriceTextChanged(string value)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price))
        {
            if (_product.Price != price)
            {
                _product.Price = price;
            }
        }
        else if (string.IsNullOrWhiteSpace(value))
        {
            if (_product.Price != 0)
            {
                _product.Price = 0;
            }
        }
        else
        {
            if (_product.Price != 0)
            {
                _product.Price = 0;
            }
        }
    }

    [RelayCommand]
    public void NavigateToList()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
    }

    [RelayCommand]
    public async Task Save()
    {
        try
        {
            ErrorMessage = null!;

            var response = await _productService.UpdateProductAsync(Product);

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
