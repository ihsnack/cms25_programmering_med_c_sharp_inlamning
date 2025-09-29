using System.Text.Json;
using Infrastructure.Factories;
using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace Infrastructure.Services;

public class ProductService(IProductRepository productRepository, IFileService fileService) : IProductService
{
    private readonly IFileService _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public async Task<ResponseResult<IEnumerable<Product>>> LoadProductsAsync()
    {
        try
        {
            var fileContent = await _fileService.LoadFromFileAsync();

            if (!fileContent.Success)
            {
                return new ResponseResult<IEnumerable<Product>>
                {
                    Success = false,
                    Message = fileContent.Message
                };
            }

            if (fileContent.Result == null || !fileContent.Result.Any())
            {
                return new ResponseResult<IEnumerable<Product>>
                {
                    Success = true,
                    Message = "File is empty or does not exist."
                };
            }

            var fileProducts = fileContent.Result;

            foreach (var product in fileProducts)
            {
                if (product is null)
                {
                    return new ResponseResult<IEnumerable<Product>>
                    {
                        Success = false,
                        Message = "File contains null product entries. File load aborted."
                    };
                }

                if (string.IsNullOrWhiteSpace(product.Title))
                {
                    return new ResponseResult<IEnumerable<Product>>
                    {
                        Success = false,
                        Message = "File contains product with empty or null title. File load aborted."
                    };
                }

                if (product.Price < 0)
                {
                    return new ResponseResult<IEnumerable<Product>>
                    {
                        Success = false,
                        Message = "Product has an invalid negative price. File load aborted."
                    };
                }
            }

            var productList = _productRepository.GetProductsFromList();

            foreach (var product in fileProducts)
            {
                if (!productList.Any(x => x.Title?.Equals(product.Title, StringComparison.OrdinalIgnoreCase) == true))
                {
                    _productRepository.AddProductToList(product);
                }
            }

            var updatedProductList = _productRepository.GetProductsFromList();

            return new ResponseResult<IEnumerable<Product>>
            {
                Success = true,
                Result = updatedProductList,
                Message = "Products loaded successfully from file."
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<IEnumerable<Product>>
            {
                Success = false,
                Message = $"{ex.Message}"
            };
        }
    }
    public async Task<ResponseResult<bool>> SaveProductsAsync()
    {
        try
        {
            var productList = _productRepository.GetProductsFromList();

            var result = await _fileService.SaveToFileAsync(productList);


            if (!result.Success)
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Products saved to file successfully.",
                Result = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = $"{ex.Message}",
                Result = false
            };
        }
    }

    public ResponseResult<IEnumerable<Product>> GetProducts()
    {
        try
        {
            var productList = _productRepository.GetProductsFromList();

            if (!productList.Any())
            {
                return new ResponseResult<IEnumerable<Product>>
                {
                    Success = true,
                    Result = productList,
                    Message = "No products in list."
                };
            }

            return new ResponseResult<IEnumerable<Product>>
            {
                Success = true,
                Result = productList,
                Message = "Products retrieved successfully."
            };

        }
        catch (Exception ex)
        {
            return new ResponseResult<IEnumerable<Product>>
            {
                Success = false,
                Message = $"{ex.Message}"
            };
        }
    }

    public ResponseResult<bool> CreateProduct(Product product)
    {
        if (product == null)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = "Product cannot be null.",
                Result = false
            };
        }
        if (string.IsNullOrWhiteSpace(product.Title))
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = "Product title cannot be empty or whitespace.",
                Result = false
            };
        }
        if (product.Price < 0)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = "Product price cannot be negative.",
                Result = false
            };
        }
        var products = _productRepository.GetProductsFromList();
        if (products.Any(p => p.Title.Equals(product.Title, StringComparison.OrdinalIgnoreCase)))
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = "A product with the same name already exists.",
                Result = false
            };
        }
        try
        {
            var newProduct = ProductFactory.Create(product.Title, product.Price, product.Category, product.Manufacturer);
            _productRepository.AddProductToList(newProduct);
            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Product was created successfully.",
                Result = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = ex.Message,
                Result = false
            };
        }
    }
    public ResponseResult<bool> UpdateProduct(string id)
    {

        try
        {
            var productToUpdate = _productRepository.GetProductById(id);

            if (productToUpdate == null)
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "Could not find product to update.",
                };

            }

            productToUpdate.Title = productToUpdate.Title;
            productToUpdate.Price = productToUpdate.Price;


            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Product was updated.",
                Result = true
            };
        }
        catch (Exception)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = "Could not update product.",
            };
        }


    }
}