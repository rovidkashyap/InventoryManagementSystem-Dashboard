using IMS_Dashboard.Models.Responses;
using IMS_Dashboard.Services.ProductServices.Interface;
using IMS_Dashboard.ViewModels.ProductsVM;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Text;

namespace IMS_Dashboard.Services.ProductServices.Service
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;
        private const string cacheKey = "ProductList";
        public ProductService(HttpClient httpClient, IMemoryCache cache, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> CreateProduct(CreateProductViewModel productViewModel)
        {
            if(productViewModel == null)
            {
                throw new ArgumentNullException(nameof(productViewModel));
            }

            try
            {
                _logger.LogInformation("Sending request to create a new product.");

                var content = new StringContent(JsonConvert.SerializeObject(productViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/products/", content);

                if(response.IsSuccessStatusCode)
                {
                    _cache.Remove(cacheKey);
                    _logger.LogInformation("Product Created Successfully.");
                    return true;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to create product. Status Code: {response.StatusCode}, Error: {errorMessage}");
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Exception in creating product: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<DisplayProductViewModel>> GetAllProducts()
        {

            // Try to get the products from cache first
            if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayProductViewModel> cachedProducts))
            {
                _logger.LogInformation("Returning cached product list.");
                return cachedProducts;
            }

            try
            {
                // Make the API call to get all products
                var response = await _httpClient.GetAsync("api/products");

                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayProductViewModel>>();

                    // Ensure we are not caching a null response
                    if(products != null)
                    {
                        // Store the result in cache for 10 minutes
                        _cache.Set(cacheKey, products, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Product list successfully retrieved and cached.");

                        return products;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null product list.");
                        return Enumerable.Empty<DisplayProductViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fetching products from API. Status Code: {response.StatusCode}");
                    throw new ProductServiceException("Failed to retrieved products from the API.");
                }
            }
            catch(HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while contacting the Product API.");
                throw new ProductServiceException("Network error occurred while fetching products.", httpEx);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GetAllProducts");
                throw new ProductServiceException("An unexpected error occurred while fetching products.", ex);
            }
        }

        public async Task<int> GetAllProductsCount()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/products/GetAllProductsCount");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                    if (responseData != null && responseData.Success)
                    {
                        return responseData.Data;
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to retrieved products count. {responseData?.Message}");
                        throw new Exception("Failed to retrieved products count.");
                    }
                }
                else
                {
                    _logger.LogError($"Error response from API, Status Code: {response.StatusCode}");
                    throw new Exception("An error occurred while contacting the API.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting products count.");
                throw;
            }
        }
    }

    public class ProductServiceException : Exception
    {
        public ProductServiceException(string message) : base(message) { }

        public ProductServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
