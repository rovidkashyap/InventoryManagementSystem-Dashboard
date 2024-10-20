using IMS_Dashboard.Models.Responses;
using IMS_Dashboard.Services.InventoryServices.Interface;
using IMS_Dashboard.Services.ProductServices.Service;
using IMS_Dashboard.ViewModels.InventoryVM;
using IMS_Dashboard.ViewModels.SuppliersVM;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace IMS_Dashboard.Services.InventoryServices.Service
{
    public class InventoryService : IInventoryService
    {
        private readonly HttpClient _httpClient;
        private ILogger<InventoryService> _logger;
        private IMemoryCache _cache;
        private const string cacheKey = "InventoryList";
        public InventoryService(HttpClient httpClient, ILogger<InventoryService> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplayInventoryViewModel>> GetAllInventories()
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayInventoryViewModel> cachedInventory))
            {
                _logger.LogInformation("Returning cached inventory list.");
                return cachedInventory;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/inventories");
                if (response.IsSuccessStatusCode)
                {
                    var inventories = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayInventoryViewModel>>();
                    if(inventories != null)
                    {
                        _cache.Set(cacheKey, inventories, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Inventories List successfully retrieved and cached.");

                        return inventories;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null inventories list.");
                        return Enumerable.Empty<DisplayInventoryViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fetching inventories from API. Status Code: {response.StatusCode}");
                    throw new InventoryServiceException("Failed to retrieved inventories from the API.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while contacting the Inventory API.");
                throw new InventoryServiceException("Network error occurred while fetching inventories.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GetAllInventories");
                throw new InventoryServiceException("An unexpected error occurred while fetching inventories.", ex);
            }
        }

        public async Task<IEnumerable<DisplayRecentInventoryViewModel>> GetRecentInventories()
        {
            var response = await _httpClient.GetAsync("api/Inventories/GetRecentInventories");
            response.EnsureSuccessStatusCode();

            var inventories = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayRecentInventoryViewModel>>();

            foreach (var inventory in inventories)
            {
                // Parse the date from the format YYY-dd-MMM
                inventory.TransactionDate = DateTime.ParseExact(
                    inventory.TransactionDate.ToString("yyyy-dd-MMM"),
                    "yyyy-dd-MMM",
                    CultureInfo.InvariantCulture
                    );
            }

            return inventories;
        }

        public async Task<int> GetAllInventoryCount()
        {
            if(!_cache.TryGetValue("inventoryCount", out int inventoryCount))
            {
                try
                {
                    var response = await _httpClient.GetAsync("api/inventories/GetAllInventoryCount");
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                        if (responseData != null && responseData.Success)
                        {
                            inventoryCount = responseData.Data;

                            _cache.Set("inventoryCount", inventoryCount, TimeSpan.FromMinutes(5));

                            return inventoryCount;
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to retrieved inventory count. {responseData?.Message}");
                            throw new Exception("Failed to retrieved inventory count.");
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
                    _logger.LogError(ex, "An error occurred while getting inventory count.");
                    throw;
                }
            }

            return inventoryCount;
        }

        public async Task<bool> CreateInventory(CreateInventoryViewModel inventoryViewModel)
        {
            if (inventoryViewModel == null)
            {
                throw new ArgumentNullException(nameof(inventoryViewModel));
            }

            try
            {
                // Calculate QuantityInStock
                int quantityInStock = inventoryViewModel.QuantityAdded - inventoryViewModel.QuantityRemoved;
                quantityInStock = Math.Max(quantityInStock, 0); // Prevent nagative stock

                var inventoryToCreate = new CreateInventoryViewModel
                {
                    TransactionTypeId = inventoryViewModel.TransactionTypeId,
                    ProductId = inventoryViewModel.ProductId,
                    SupplierId = inventoryViewModel.SupplierId,
                    QuantityAdded = inventoryViewModel.QuantityAdded,
                    QuantityRemoved = inventoryViewModel.QuantityRemoved,
                    QuantityInStock = quantityInStock,
                    UnitPrice = inventoryViewModel.UnitPrice,
                    Remark = inventoryViewModel.Remark,
                    TransactionDate = inventoryViewModel.TransactionDate,
                };

                _logger.LogInformation("Sending request to create a new inventory.");

                var content = new StringContent(JsonConvert.SerializeObject(inventoryToCreate), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/inventories", content);

                if (response.IsSuccessStatusCode)
                {
                    _cache.Remove(cacheKey);
                    _logger.LogInformation("Inventory created successfully.");

                    return true;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Failed to create inventory: {response.StatusCode}, Error: {message}");

                    _logger.LogWarning($"Request content: {JsonConvert.SerializeObject(inventoryToCreate)}");

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the inventory.");
                return false;
            }
        }
    }

    public class InventoryServiceException : Exception
    {
        public InventoryServiceException(string message) : base(message) { }

        public InventoryServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
