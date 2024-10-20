using IMS_Dashboard.Models.Responses;
using IMS_Dashboard.Services.CustomerServices.Interface;
using IMS_Dashboard.ViewModels.CustomersVM;
using Microsoft.Extensions.Caching.Memory;

namespace IMS_Dashboard.Services.CustomerServices.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CustomerService> _logger;
        private const string cacheKey = "CustomerList";
        public CustomerService(HttpClient httpClient, IMemoryCache cache, ILogger<CustomerService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CreateCustomerViewModel> CreateCustomerAsync(CreateCustomerViewModel customerViewModel)
        {
            if(customerViewModel == null)
            {
                throw new ArgumentNullException(nameof(customerViewModel));
            }

            try
            {
                _logger.LogInformation("Sending request to create a new customer.");

                var response = await _httpClient.PostAsJsonAsync("api/customers", customerViewModel);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize response into CreateCustomerViewModel
                    var createdCustomer = await response.Content.ReadFromJsonAsync<CreateCustomerViewModel>();

                    // Remove the cached customers
                    _cache.Remove(cacheKey);
                    _logger.LogInformation($"Customer created successfully with ID: {createdCustomer.CustomerId}");

                    return createdCustomer;
                }
                else
                {
                    _logger.LogWarning($"Failed to create customer: {response.StatusCode}");
                    throw new Exception($"Error while creating customer. Status Code: {response.StatusCode}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the customer.");
                throw;  // Re throw exception to be handled at higher level
            }
        }

        public async Task<IEnumerable<DisplayCustomerViewModel>> GetAllCustomers()
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayCustomerViewModel> cachedCustomers))
            {
                _logger.LogInformation("Returning cached customers List.");
                return cachedCustomers;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/customers");

                if(response.IsSuccessStatusCode)
                {
                    var customers = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayCustomerViewModel>>();

                    if(customers != null)
                    {
                        _cache.Set(cacheKey, customers, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Customers list successfully retrieved and cached.");

                        return customers;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null customers list.");
                        return Enumerable.Empty<DisplayCustomerViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fetching customers from API. Status Code {response.StatusCode}");
                    throw new CustomerServiceException("Failed to retrieved customers from the API.");
                }
            }
            catch(HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while fethcing Cusstomer API.");
                throw new CustomerServiceException("Network error occurred while fetching customers.", httpEx);
            }
            catch(Exception ex)
            {
                _logger.LogError("An unexpected error occurred in GetAllCustomers");
                throw new CustomerServiceException("An unexpected error occurred while fetching customers List.");
            }
        }

        public async Task<int> GetAllCustomersCountAsync()
        {
            if(!_cache.TryGetValue("CustomerCount", out int customerCount))
            {
                try
                {
                    var response = await _httpClient.GetAsync("api/Customers/GetAllCustomersCount");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                        if (responseData != null && responseData.Success)
                        {
                            customerCount = responseData.Data;

                            // cache the result for 5 minutes
                            _cache.Set("CustomerCount", customerCount, TimeSpan.FromMinutes(5));

                            return customerCount;
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to retrieve customer count. Message: {responseData?.Message}");
                            throw new Exception("Failed to retrieve customer count.");
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
                    _logger.LogError(ex, "An error occurred while getting the customer count.");
                    throw;
                }
            }

            // Return cache value if availale
            return customerCount;
        }
    }

    public class CustomerServiceException : Exception
    {
        public CustomerServiceException(string message) : base(message) { }

        public CustomerServiceException(string message, Exception innerException)
            :base(message, innerException) { }
    }
}
