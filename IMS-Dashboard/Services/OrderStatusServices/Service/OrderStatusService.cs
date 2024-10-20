using Humanizer;
using IMS_Dashboard.Services.OrderStatusServices.Interface;
using IMS_Dashboard.ViewModels.OrderStatusVM;
using Microsoft.Extensions.Caching.Memory;

namespace IMS_Dashboard.Services.OrderStatusServices.Service
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IHttpClientFactory _httpClientFacatory;
        private readonly ILogger<OrderStatusService> _logger;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private const string cacheKey = "OrderStatusList";
        public OrderStatusService(IHttpClientFactory httpClientFactory, ILogger<OrderStatusService> logger, 
            IMemoryCache cache)
        {
            _httpClientFacatory = httpClientFactory;
            _logger = logger;
            _cache = cache;
            _httpClient = _httpClientFacatory.CreateClient("OrderStatusClient");
        }

        public async Task<IEnumerable<DisplayOrderStatusViewModel>> GetAllOrderStatus()
        {
            if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayOrderStatusViewModel> cachedOrderStatus))
            {
                _logger.LogInformation("Returned cached orderstatus list.");
                return cachedOrderStatus;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/orderstatus/");
                if(response.IsSuccessStatusCode)
                {
                    var orderstatus = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayOrderStatusViewModel>>();
                    if(orderstatus != null)
                    {
                        _cache.Set(cacheKey, orderstatus, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Order Status successfully retrieved and cached.");

                        return orderstatus;
                    }
                    else
                    {
                        _logger.LogError("The API returned null orderstatus list.");
                        return Enumerable.Empty<DisplayOrderStatusViewModel>();
                    }
                }
                else
                {
                    _logger.LogWarning($"Error fetching orderstatus from API. Status Code: {response.StatusCode}");
                    throw new Exception("Failed to retrieved orderstatus from the API.");
                }
            }
            catch(HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while fetching orderstatus API.");
                throw new Exception("Network error occurred while fetching orderstatus.");
            }
            catch(Exception ex)
            {
                _logger.LogError("An unexpected error occurred in GetAllOrderStatus.");
                throw new Exception("An unexpected error occurred while fetching the orderstatus list.");
            }
        }
    }
}
