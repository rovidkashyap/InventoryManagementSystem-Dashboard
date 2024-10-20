using IMS_Dashboard.Models.Responses;
using IMS_Dashboard.Services.SupplierServices.Interface;
using IMS_Dashboard.ViewModels.CustomersVM;
using IMS_Dashboard.ViewModels.SuppliersVM;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IMS_Dashboard.Services.SupplierServices.Service
{
    public class SupplierService : ISupplierService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SupplierService> _logger;
        private readonly IMemoryCache _cache;
        private const string cacheKey = "SupplierList";
        public SupplierService(HttpClient httpClient, ILogger<SupplierService> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplaySupplierViewModel>> GetAllSuppliers()
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplaySupplierViewModel> cachedSuppliers))
            {
                _logger.LogInformation("Returning cached supplier list.");
                return cachedSuppliers;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/suppliers");
                if(response.IsSuccessStatusCode)
                {
                    var suppliers = await response.Content.ReadFromJsonAsync<IEnumerable<DisplaySupplierViewModel>>();
                    if(suppliers != null)
                    {
                        _cache.Set(cacheKey, suppliers, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Suppliers List successfully retrieved and cached.");

                        return suppliers;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null suppliers list.");
                        return Enumerable.Empty<DisplaySupplierViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fetching suppliers from API. Status Code: {response.StatusCode}");
                    throw new SupplierServiceException("Failed to retrieved suppliers from the API.");
                }
            }
            catch(HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while contacting the supplier API.");
                throw new SupplierServiceException("Network error occurred while fetching suppliers.", httpEx);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GetAllSuppliers");
                throw new SupplierServiceException("An unexpected error occurred while fetching suppliers.", ex);
            }
        }

        public async Task<CreateSupplierViewModel> CreateSupplier(CreateSupplierViewModel supplierViewModel)
        {
            if(supplierViewModel == null)
            {
                throw new ArgumentNullException(nameof(supplierViewModel));
            }

            try
            {
                _logger.LogInformation("Sending request to create a new supplier.");

                var response = await _httpClient.PostAsJsonAsync("api/suppliers", supplierViewModel);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize response into CreateSupplierViewModel
                    var createdSupplier = await response.Content.ReadFromJsonAsync<CreateSupplierViewModel>();

                    _cache.Remove(cacheKey);
                    _logger.LogInformation($"Supplier created successfully with ID: {createdSupplier.SupplierId}");

                    return createdSupplier;
                }
                else
                {
                    _logger.LogWarning($"Failed to create supplier: {response.StatusCode}");
                    throw new Exception($"Error while creating supplier. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the customer.");
                throw;  // Re throw exception to be handled at higher level
            }
        }

        public async Task<int> GetAllSuppliersCountAsync()
        {
            if (!_cache.TryGetValue("supplierCount", out int supplierCount))
            {
                try
                {
                    var response = await _httpClient.GetAsync("api/suppliers/GetAllSuppliersCount");
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                        if (responseData != null && responseData.Success)
                        {
                            supplierCount = responseData.Data;

                            _cache.Set("supplierCount", supplierCount, TimeSpan.FromMinutes(5));

                            return supplierCount;
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to retrieved supplier count. {responseData?.Message}");
                            throw new Exception("Failed to retrieved supplier count.");
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
                    _logger.LogError(ex, "An error occurred while getting supplier count.");
                    throw;
                }
            }

            return supplierCount;
        }
    }

    public class SupplierServiceException : Exception
    {
        public SupplierServiceException(string message) : base(message) { }

        public SupplierServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
