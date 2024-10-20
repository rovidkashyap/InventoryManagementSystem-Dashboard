using IMS_Dashboard.Services.ProductServices.Service;
using IMS_Dashboard.Services.UnitOfMeasureServices.Interface;
using IMS_Dashboard.ViewModels.ProductsVM;
using IMS_Dashboard.ViewModels.SuppliersVM;
using IMS_Dashboard.ViewModels.UnitOfMeasureVM;
using Microsoft.Extensions.Caching.Memory;

namespace IMS_Dashboard.Services.UnitOfMeasureServices.Service
{
    public class UnitOfMeasureService : IUnitOfMeasureService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UnitOfMeasureService> _logger;
        private const string cacheKey = "UnitOfMeasureList";
        public UnitOfMeasureService(HttpClient httpClient, IMemoryCache cache, ILogger<UnitOfMeasureService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<DisplayUnitViewModel>> GetAllUnitofMeasures()
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayUnitViewModel> cachedUnits))
            {
                _logger.LogInformation("Returning cached units list.");
                return cachedUnits;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/UnitOfMeasures");

                if (response.IsSuccessStatusCode)
                {
                    var units = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayUnitViewModel>>();

                    if (units != null)
                    {
                        _cache.Set(cacheKey, units, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Units list successfully retrieved and cached.");

                        return units;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null units list.");
                        return Enumerable.Empty<DisplayUnitViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fetching units from API. Status Code: {response.StatusCode}");
                    throw new UnitServiceException("Failed to retrieved units from the API.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while contacting the Units API.");
                throw new UnitServiceException("Network error occurred while fetching units.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GetAllUnitOfMeasures");
                throw new UnitServiceException("An unexpected error occurred while fetching units.", ex);
            }
        }

        public async Task<CreateUnitOfMeasureViewModel> CreateUnit(CreateUnitOfMeasureViewModel unitViewModel)
        {
            if(unitViewModel == null)
            {
                throw new ArgumentNullException(nameof(unitViewModel));
            }

            try
            {
                _logger.LogInformation("Sending request to create a new unit.");

                var response = await _httpClient.PostAsJsonAsync("api/unitofmeasures", unitViewModel);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize response into CreateSupplierViewModel
                    var createdUnits = await response.Content.ReadFromJsonAsync<CreateUnitOfMeasureViewModel>();

                    _cache.Remove(cacheKey);
                    _logger.LogInformation($"Unit of Measure created successfully with ID: {createdUnits.UnitOfMeasureId}");

                    return createdUnits;
                }
                else
                {
                    _logger.LogWarning($"Failed to create units: {response.StatusCode}");
                    throw new Exception($"Error while creating units. Status Code: {response.StatusCode}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the units.");
                throw;  // Re throw exception to be handled at higher level
            }
        }
    }

    public class UnitServiceException : Exception
    {
        public UnitServiceException(string message) : base(message) { }
        public UnitServiceException(string message, Exception innerException) { }
    }
}
