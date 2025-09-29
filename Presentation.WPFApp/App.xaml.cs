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

                services.AddTransient<ProductListViewModel>();
                services.AddTransient<ProductListView>();

                services.AddTransient<ProductAddViewModel>();
                services.AddTransient<ProductAddView>();

                services.AddSingleton<ProductDetailsViewModel>();
                services.AddSingleton<ProductDetailsView>();

                services.AddSingleton<ProductEditViewModel>();
                services.AddSingleton<ProductEditView>();
            })
            .Build();
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var productService = _host.Services.GetRequiredService<IProductService>();

        var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _host.Services.GetRequiredService<ProductListViewModel>();

        var window = _host.Services.GetRequiredService<MainWindow>();
        window.Show();
    }
}