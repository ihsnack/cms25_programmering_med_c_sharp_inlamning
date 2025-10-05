using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class ProductDetailsViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
{

    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;

    [ObservableProperty]
    public Product _product = new();

    [ObservableProperty]
    public string _errorMessage = null!;

    [RelayCommand]
    public void NavigateToList()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
    }

    [RelayCommand]
    public void NavigateToProductEdit()
    {
        var productEditView = _serviceProvider.GetRequiredService<ProductEditViewModel>();
        productEditView.Product = Product;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productEditView;
    }

    [RelayCommand]
    public async Task Delete()
    {
        try
        {
            ErrorMessage = null!;

            var response = await _productService.RemoveProductAsync(Product.Id);

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
