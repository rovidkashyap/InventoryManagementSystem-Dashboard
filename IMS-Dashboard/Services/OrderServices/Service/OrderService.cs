using IMS_Dashboard.Models.Responses;
using IMS_Dashboard.Services.OrderServices.Interface;
using IMS_Dashboard.ViewModels.OrderItemVM;
using IMS_Dashboard.ViewModels.OrderVM;
using Microsoft.Extensions.Caching.Memory;

namespace IMS_Dashboard.Services.OrderServices.Service
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrderService> _logger;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        public OrderService(IHttpClientFactory httpClientFactory, ILogger<OrderService> logger, 
            IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = cache;
            _httpClient = _httpClientFactory.CreateClient("OrderClient");   // Use named client
        }

        public async Task<int> GetAllOrdersCount()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/orders/GetAllOrdersCount");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                    if (responseData != null && responseData.Success)
                    {
                        return responseData.Data;
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to retrieved order count. {responseData?.Message}");
                        return 0;
                    }
                }
                else
                {
                    _logger.LogError($"Error response from API, Status Code: {response.StatusCode}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting order count.");
                return 0;
            }
        }

        public async Task<int> GetOrdersCountByOrderStatus(string statusName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Orders/GetOrdersCountByStatusName?statusName={statusName}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                    if(responseData != null && responseData.Success)
                    {
                        return responseData.Data;
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to retrieved order count for status {statusName}");
                        return 0;
                    }
                }
                else
                {
                    _logger.LogError($"Error response from API for status {statusName}, Status Code: {response.StatusCode}");
                    return 0;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting order count for status {statusName}");
                return 0;
            }
        }

        public async Task<IEnumerable<DisplayOrdersWithOrderItemsViewModel>> GetAllOrdersWithOrderItems()
        {
            const string cacheKey = "OrderList";
            
            if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayOrdersWithOrderItemsViewModel> cachedOrder))
            {
                _logger.LogInformation("Returned cached order list.");
                return cachedOrder;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/orders/GetAllOrdersWithOrderItems");
                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayOrdersWithOrderItemsViewModel>>();
                    if(orders != null)
                    {
                        _cache.Set(cacheKey, orders, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Orders List successfully rerieved and cached.");

                        return orders;
                    }
                    else
                    {
                        _logger.LogError("The API returned null orders list.");
                        return Enumerable.Empty<DisplayOrdersWithOrderItemsViewModel>();
                    }
                }
                else
                {
                    _logger.LogWarning($"Error fetching orders from API. Status Code: {response.StatusCode}");
                    throw new Exception("Failed to retrieved orders from the API.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while fethcing Ordes API.");
                throw new Exception("Network error occurred while fetching orders.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred in GetAllOrdersWithOrderItems");
                throw new Exception("An unexpected error occurred while fetching orders List.");
            }
        }

        public async Task<IEnumerable<DisplayRecentOrdersViewModel>> GetRecentOrders(int count)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/orders/GetRecentOrders?count={count}");
                response.EnsureSuccessStatusCode();

                var orders = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayRecentOrdersViewModel>>();
                return orders;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching recent orders.");
                throw;
            }
        }
    }
}
