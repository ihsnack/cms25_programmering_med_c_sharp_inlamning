using System.Text.Json;
using System.Threading.Tasks;
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


                if (string.IsNullOrWhiteSpace(product.Category.Name))
                {
                    return new ResponseResult<IEnumerable<Product>>
                    {
                        Success = false,
                        Message = "Product category name needs to be provided. File load aborted.",

                    };
                }

                if (string.IsNullOrWhiteSpace(product.Manufacturer.Name))
                {
                    return new ResponseResult<IEnumerable<Product>>
                    {
                        Success = false,
                        Message = "Product manufacturer name needs to be provided. File load aborted.",
                    };
                }
            }

            _productRepository.SetProductsToList(fileProducts);

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

    public async Task<ResponseResult<bool>> CreateProduct(Product product)
    {
        try
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

            if (string.IsNullOrWhiteSpace(product.Category.Name))
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "Product category name needs to be provided",
                    Result = false
                };
            }

            if (string.IsNullOrWhiteSpace(product.Manufacturer.Name))
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "Product manufacturer name needs to be provided",
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

            var newProduct = ProductFactory.Create(product.Title, product.Price, product.Category, product.Manufacturer);
            _productRepository.AddProductToList(newProduct);

            var saveResponse = await SaveProductsAsync();

            if (!saveResponse.Success)
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = $"{saveResponse.Message}",
                };
            }

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
    public async Task<ResponseResult<bool>> UpdateProductAsync(Product product)
    {
        try
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

            if (string.IsNullOrWhiteSpace(product.Category.Name))
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "Product category name needs to be provided",
                    Result = false
                };
            }

            if (string.IsNullOrWhiteSpace(product.Manufacturer.Name))
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "Product manufacturer name needs to be provided",
                    Result = false
                };
            }

            var products = _productRepository.GetProductsFromList();
            if (products.Any(p => p.Id != product.Id && p.Title.Equals(product.Title, StringComparison.OrdinalIgnoreCase)))
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "A product with the same name already exists.",
                    Result = false
                };
            }

            var productToUpdate = _productRepository.GetProductByIdFromList(product.Id);

            if (productToUpdate == null)
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "Could not find product to update.",
                };

            }

            productToUpdate.Title = product.Title;
            productToUpdate.Price = product.Price;
            productToUpdate.Category = product.Category;
            productToUpdate.Manufacturer = product.Manufacturer;

            var saveResponse = await SaveProductsAsync();

            if (!saveResponse.Success)
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = $"{saveResponse.Message}",
                };
            }

            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Product was updated.",
                Result = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Message = ex.Message,
            };
        }
    }

    public async Task<ResponseResult<bool>> RemoveProduct(string id)
    {

        try
        {
            var result = _productRepository.RemoveProductFromList(id);

            if (result > 0)
            {

                var saveReponse = await SaveProductsAsync();

                if (!saveReponse.Success)
                {
                    return new ResponseResult<bool>
                    {
                        Success = false,
                        Message = $"{saveReponse.Message}",
                    };
                }

                return new ResponseResult<bool>
                {
                    Success = true,
                    Message = "Removed product successfully.",
                };
            }
            else
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "Could not find product to remove.",
                };
            }
        }
        catch (Exception)
        {

            return new ResponseResult<bool>
            {
                Success = false,
                Message = "Could not perform removal of product.",
            };
        }

    }
}