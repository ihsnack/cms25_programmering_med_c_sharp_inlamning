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

    public ProductListViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        Task.Run(PopulateProductListAsync);
    }

    private async Task PopulateProductListAsync()
    {
        var productService = _serviceProvider.GetRequiredService<IProductService>();
        var products = await productService.LoadProductsAsync();
        var result = products.Result;
        ProductList = new ObservableCollection<Product>(result!);
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
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductDetailsViewModel>();
    }
}
