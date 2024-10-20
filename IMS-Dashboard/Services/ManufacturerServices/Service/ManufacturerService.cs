using IMS_Dashboard.Services.ManufacturerServices.Interface;
using IMS_Dashboard.Services.ProductServices.Service;
using IMS_Dashboard.ViewModels.ManufacturerVM;
using IMS_Dashboard.ViewModels.SuppliersVM;
using Microsoft.Extensions.Caching.Memory;

namespace IMS_Dashboard.Services.ManufacturerServices.Service
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ManufacturerService> _logger;
        private const string cacheKey = "ManufacturerList";
        public ManufacturerService(HttpClient httpClient, IMemoryCache cache, ILogger<ManufacturerService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<DisplayManufacturerViewModel>> GetAllManufacturers()
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayManufacturerViewModel> cachedManufacturer))
            {
                _logger.LogInformation("Returning cached product list.");
                return cachedManufacturer;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/manufacturers");
                if(response.IsSuccessStatusCode)
                {
                    var manufacturers = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayManufacturerViewModel>>();
                    if(manufacturers != null)
                    {
                        _cache.Set(cacheKey, manufacturers, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Manufacturer list successfully retrieved and cached.");

                        return manufacturers;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null manufacturer list.");
                        return Enumerable.Empty<DisplayManufacturerViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fetching manufacturers from API. Status Code: {response.StatusCode}");
                    throw new ProductServiceException("Failed to retrieved manufacturers from the API.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while contacting the Manufacturer API.");
                throw new ManufacturerServiceException("Network error occurred while fetching manufacturer.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GetAllManufacturers.");
                throw new ManufacturerServiceException("An unexpected error occurred while fetching manufacturers.", ex);
            }
        }

        public async Task<CreateManufacturerViewModel> CreateManufacturer(CreateManufacturerViewModel manufacturerViewModel)
        {
            if (manufacturerViewModel == null)
            {
                throw new ArgumentNullException(nameof(manufacturerViewModel));
            }

            try
            {
                _logger.LogInformation("Sending request to create a new manufacturer.");

                var response = await _httpClient.PostAsJsonAsync("api/manufacturers", manufacturerViewModel);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize response into CreateManufacturerViewModel
                    var createdManufacturer = await response.Content.ReadFromJsonAsync<CreateManufacturerViewModel>();

                    _cache.Remove(cacheKey);
                    _logger.LogInformation($"Manufacturer created successfully with ID: {createdManufacturer.ManufacturerId}");

                    return createdManufacturer;
                }
                else
                {
                    _logger.LogWarning($"Failed to create manufacturer: {response.StatusCode}");
                    throw new Exception($"Error while creating manufacturer. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the manufacturer.");
                throw;  // Re throw exception to be handled at higher level
            }
        }
    }

    public class ManufacturerServiceException : Exception
    {
        public ManufacturerServiceException(string message) : base(message) { }

        public ManufacturerServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
