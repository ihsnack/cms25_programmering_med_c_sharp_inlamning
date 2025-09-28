using System.Windows;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.WPFApp;
using Presentation.WPFApp.ViewModels;
using Presentation.WPFApp.Views;

namespace Presentation.WPFApp;

public partial class App : Application
{
    private IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IFileRepository, FileRepository>();
                services.AddScoped<IFileService, FileService>();
                services.AddScoped<IProductRepository, ProductRepository>();
                services.AddScoped<IProductService, ProductService>();

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainViewModel>();

                services.AddTransient<ListProductViewModel>();
                services.AddTransient<ListProductView>();

                services.AddTransient<AddProductViewModel>();
                services.AddTransient<AddProductView>();

                services.AddSingleton<ProductDetailsViewModel>();
                services.AddSingleton<ProductDetailsView>();
            })
            .Build();
    }


    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var productService = _host.Services.GetRequiredService<IProductService>();
        await productService.LoadProductsAsync();

        var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _host.Services.GetRequiredService<ListProductViewModel>();

        var window = _host.Services.GetRequiredService<MainWindow>();
        window.Show();
    }
}