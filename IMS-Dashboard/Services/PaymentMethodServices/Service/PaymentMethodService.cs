using IMS_Dashboard.Services.PaymentMethodServices.Interface;
using IMS_Dashboard.Services.ProductServices.Service;
using IMS_Dashboard.ViewModels.PaymentMethodsVM;
using IMS_Dashboard.ViewModels.ProductsVM;
using IMS_Dashboard.ViewModels.SuppliersVM;
using Microsoft.Extensions.Caching.Memory;

namespace IMS_Dashboard.Services.PaymentMethodServices.Service
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PaymentMethodService> _logger;
        private const string cacheKey = "PaymentMethodList";
        public PaymentMethodService(HttpClient httpClient, IMemoryCache cache, ILogger<PaymentMethodService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<DisplayPaymentMethodViewModel>> GetAllPaymentMethods()
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayPaymentMethodViewModel> cachedpaymentMethod))
            {
                _logger.LogInformation("Returning cached PaymentMethod list.");
                return cachedpaymentMethod;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/paymentmethods");

                if (response.IsSuccessStatusCode)
                {
                    var paymentMethod = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayPaymentMethodViewModel>>();

                    if (paymentMethod != null)
                    {
                        _cache.Set(cacheKey, paymentMethod, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("PaymentMethods list successfully retrieved and cached.");

                        return paymentMethod;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null paymentMethod list.");
                        return Enumerable.Empty<DisplayPaymentMethodViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fetching paymentmethods from API. Status Code: {response.StatusCode}");
                    throw new PaymentMethodServiceException("Failed to retrieved paymentmethods from the API.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while contacting the paymentmethods API.");
                throw new PaymentMethodServiceException("Network error occurred while fetching paymentmethods.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GetAllPaymentMethods");
                throw new PaymentMethodServiceException("An unexpected error occurred while fetching paymentmethods.", ex);
            }
        }

        public async Task<CreatePaymentMethodViewModel> CreatePaymentMethod(CreatePaymentMethodViewModel paymentViewModel)
        {
            if (paymentViewModel == null)
            {
                throw new ArgumentNullException(nameof(paymentViewModel));
            }

            try
            {
                _logger.LogInformation("Sending request to create a new payment method.");

                var response = await _httpClient.PostAsJsonAsync("api/paymentmethods", paymentViewModel);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize response into CreatePaymentMethodViewModel
                    var createdPaymentMethod = await response.Content.ReadFromJsonAsync<CreatePaymentMethodViewModel>();

                    _cache.Remove(cacheKey);
                    _logger.LogInformation($"Payment Method created successfully with ID: {createdPaymentMethod.PaymentMethodId}");

                    return createdPaymentMethod;
                }
                else
                {
                    _logger.LogWarning($"Failed to create payment method: {response.StatusCode}");
                    throw new Exception($"Error while creating payment method. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the payment method.");
                throw;  // Re throw exception to be handled at higher level
            }
        }
    }

    public class PaymentMethodServiceException : Exception
    {
        public PaymentMethodServiceException(string message) : base(message) { }

        public PaymentMethodServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
