using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.Dialogs;
using Presentation.Interfaces;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((config, services) =>
    {
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFileService, FileService>();
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IMenuDialogs, MenuDialogs>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var menuDialogs = scope.ServiceProvider.GetRequiredService<IMenuDialogs>();

    await menuDialogs.MenuOptionsDialogAsync();
}