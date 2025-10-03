using System.Globalization;
using System.Threading.Tasks;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Presentation.Interfaces;

namespace Presentation.Dialogs;

public class MenuDialogs : IMenuDialogs
{
    private readonly IProductService _productService;

    public MenuDialogs(IProductService products)
    {
        _productService = products;
    }

    public async Task MenuOptionsDialogAsync()
    {
        while (true)
        {
            Dialogs.MenuHeading("MAIN MENU");
            Console.WriteLine("1. New Product");
            Console.WriteLine("2. View Products");
            Console.WriteLine("3. Load Products From File");
            Console.WriteLine("4. Save Products To File");
            Console.WriteLine("0. Exit Program");
            Console.WriteLine();
            var option = Dialogs.Prompt("Select option: ");

            switch (option)
            {
                case "1":
                    await AddProductDialogAsync();
                    break;
                case "2":
                    ViewProductsDialogAsync();
                    break;
                case "3":
                    await ViewLoadProductsFromFileAsync();
                    break;
                case "4":
                    await ViewSaveListToFileAsync();
                    break;
                case "0":
                    Environment.Exit(0);
                    return;
                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option. Please try again. Press any key.");
                    Console.WriteLine();
                    Console.ReadKey();
                    break;
            }
        }
    }

    public async Task AddProductDialogAsync()
    {
        try
        {
            Dialogs.MenuHeading("NEW PRODUCT");

            string title;
            do
            {
                title = Dialogs.Prompt("Enter Title: ");

                if (!ProductValidationHelper.IsValidTitle(title))
                {
                    Console.WriteLine();
                    Console.WriteLine("Title cannot be empty. Please try again. Press any key.");
                    Console.WriteLine();
                }
            } while (!ProductValidationHelper.IsValidTitle(title));

            Console.WriteLine();

            decimal priceValue;
            string priceInput;

            do
            {
                priceInput = Dialogs.Prompt("Enter Price (use dot as decimal separator): ");

                if (!ProductValidationHelper.TryParsePrice(priceInput, out priceValue))
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter a valid price greater than zero. Press any key.");
                    Console.WriteLine();
                    Console.ReadKey();
                }
            } while (!ProductValidationHelper.TryParsePrice(priceInput, out priceValue));

            Console.WriteLine();

            string categoryName;
            do
            {
                categoryName = Dialogs.Prompt("Enter Category name: ");
                if (!ProductValidationHelper.IsValidCategoryName(categoryName))
                {
                    Console.WriteLine();
                    Console.WriteLine("Category name cannot be empty. Please try again. Press any key.");
                    Console.WriteLine();
                    Console.ReadKey();
                }
            } while (!ProductValidationHelper.IsValidCategoryName(categoryName));

            Category category = new Category { Name = categoryName };

            Console.WriteLine();

            string manufacturerName;
            do
            {
                manufacturerName = Dialogs.Prompt("Enter Manufacturer name: ");
                if (!ProductValidationHelper.IsValidManufacturerName(manufacturerName))
                {
                    Console.WriteLine();
                    Console.WriteLine("Manufacturer name cannot be empty. Please try again. Press any key.");
                    Console.WriteLine();
                    Console.ReadKey();
                }
            } while (!ProductValidationHelper.IsValidManufacturerName(manufacturerName));

            Manufacturer manufacturer = new Manufacturer { Name = manufacturerName };

            var product = new Product { Title = title, Price = priceValue, Category = category, Manufacturer = manufacturer };

            var response = await _productService.CreateProduct(product);

            Console.WriteLine();
            Console.WriteLine($"{response.Message} Press any key.");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }

    public void ViewProductsDialogAsync()
    {
        try
        {
            Dialogs.MenuHeading("PRODUCTS");
            var response = _productService.GetProducts();
            var products = response.Result ?? [];
            if (products.Any())
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"{"Id:",-15}{product.Id}");
                    Console.WriteLine($"{"Title:",-15}{product.Title}");
                    Console.WriteLine($"{"Price:",-15}{product.Price.ToString(CultureInfo.InvariantCulture)}");
                    Console.WriteLine($"{"Category:",-15}{product.Category.Name}");
                    Console.WriteLine($"{"Manufacturer:",-15}{product.Manufacturer.Name}");
                    if (product != products.Last())
                    {
                        Console.WriteLine();
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine($"{response.Message} Press any key.");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }

    public async Task ViewLoadProductsFromFileAsync()
    {
        try
        {
            Dialogs.MenuHeading("LOAD PRODUCTS FROM FILE");
            var response = await _productService.LoadProductsAsync();
            Console.WriteLine($"{response.Message} Press any key.");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }

    public async Task ViewSaveListToFileAsync()
    {
        try
        {
            Dialogs.MenuHeading("SAVE LIST TO FILE");
            var response = await _productService.SaveProductsAsync();
            Console.WriteLine($"{response.Message} Press any key.");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}