using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPFApp.ViewModels;

public partial class ProductDetailsViewModel(IServiceProvider serviceProvider) : ObservableObject
{

    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [ObservableProperty]
    private Product _product = new();

    [RelayCommand]
    public void NavigateToList()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ListProductViewModel>();
    }
}
