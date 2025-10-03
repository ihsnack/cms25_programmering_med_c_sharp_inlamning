using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class ProductListViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<Product> _productList = [];

    [ObservableProperty]
    private string _errorMessage = null!;

    public ProductListViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        Task.Run(PopulateProductListAsync);
    }

    private async Task PopulateProductListAsync()
    {
        try
        {
            ErrorMessage = null!;

            var productService = _serviceProvider.GetRequiredService<IProductService>();
            var response = await productService.LoadProductsAsync();

            if (!response.Success)
            {
                ErrorMessage = response.Message!;
                ProductList = new ObservableCollection<Product>();
                return;
            }

            var result = response.Result;
            ProductList = new ObservableCollection<Product>(result!);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            ProductList = new ObservableCollection<Product>();
        }
    }

    [RelayCommand]
    public void NavigateToAddProduct()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductAddViewModel>();
    }


    [RelayCommand]
    public void NavigateToProductDetails(Product product)
    {
        var productDetailsView = _serviceProvider.GetRequiredService<ProductDetailsViewModel>();
        productDetailsView.Product = product;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productDetailsView;
    }
}
