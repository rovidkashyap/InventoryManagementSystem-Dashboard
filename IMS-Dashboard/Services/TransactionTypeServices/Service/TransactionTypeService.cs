using IMS_Dashboard.Services.ProductServices.Service;
using IMS_Dashboard.Services.TransactionTypeServices.Interface;
using IMS_Dashboard.ViewModels.ProductsVM;
using IMS_Dashboard.ViewModels.SuppliersVM;
using IMS_Dashboard.ViewModels.TransactionTypeVM;
using Microsoft.Extensions.Caching.Memory;

namespace IMS_Dashboard.Services.TransactionTypeServices.Service
{
    public class TransactionTypeService : ITransactionTypeService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TransactionTypeService> _logger;
        private const string cacheKey = "TransactionTypeList";
        public TransactionTypeService(HttpClient httpClient, IMemoryCache cache, ILogger<TransactionTypeService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<DisplayTransactionTypeViewModel>> GetAllTransactionTypes()
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayTransactionTypeViewModel> cachedTransactionType))
            {
                _logger.LogInformation("Returning cached TransactionType list.");
                return cachedTransactionType;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/TransactionTypes");

                if (response.IsSuccessStatusCode)
                {
                    var transactionType = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayTransactionTypeViewModel>>();

                    if (transactionType != null)
                    {
                        _cache.Set(cacheKey, transactionType, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("TransactionType list successfully retrieved and cached.");

                        return transactionType;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null transactionType list.");
                        return Enumerable.Empty<DisplayTransactionTypeViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fetching transactionType from API. Status Code: {response.StatusCode}");
                    throw new TransactionTypeServiceException("Failed to retrieved transactionType from the API.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while contacting the transactionType API.");
                throw new TransactionTypeServiceException("Network error occurred while fetching transactionType.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GetAllProducts");
                throw new TransactionTypeServiceException("An unexpected error occurred while fetching transactionType.", ex);
            }
        }

        public async Task<CreateTransactionTypeViewModel> CreateTransactionType(CreateTransactionTypeViewModel transactionViewModel)
        {
            if (transactionViewModel == null)
            {
                throw new ArgumentNullException(nameof(transactionViewModel));
            }

            try
            {
                _logger.LogInformation("Sending request to create a new transaction type.");

                var response = await _httpClient.PostAsJsonAsync("api/transactiontypes", transactionViewModel);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize response into CreateTransactionTypeViewModel
                    var createdTransaction = await response.Content.ReadFromJsonAsync<CreateTransactionTypeViewModel>();

                    _cache.Remove(cacheKey);
                    _logger.LogInformation($"Transaction Type created successfully with ID: {createdTransaction.TransactionTypeId}");

                    return createdTransaction;
                }
                else
                {
                    _logger.LogWarning($"Failed to create transaction Type: {response.StatusCode}");
                    throw new Exception($"Error while creating transaction Type. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the transaction Type.");
                throw;  // Re throw exception to be handled at higher level
            }
        }
    }

    public class TransactionTypeServiceException : Exception
    {
        public TransactionTypeServiceException(string message)
            : base(message) { }

        public TransactionTypeServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
