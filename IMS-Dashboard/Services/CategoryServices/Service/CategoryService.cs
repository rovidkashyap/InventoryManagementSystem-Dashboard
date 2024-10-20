using IMS_Dashboard.Services.CategoryServices.Interface;
using IMS_Dashboard.ViewModels.CategoryVM;
using Microsoft.Extensions.Caching.Memory;

namespace IMS_Dashboard.Services.CategoryServices.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CategoryService> _logger;
        private const string cacheKey = "CategoryList";
        public CategoryService(HttpClient httpClient, IMemoryCache cache, ILogger<CategoryService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<DisplayCategoryViewModel>> GetAllCategories()
        {

            if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayCategoryViewModel> cachedCategory))
            {
                _logger.LogInformation("Returning cached category list.");
                return cachedCategory;
            }

            try
            {
                var response = await _httpClient.GetAsync("api/categories");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayCategoryViewModel>>();
                    if(categories != null)
                    {
                        _cache.Set(cacheKey, categories, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("Catgories list sccessfully retrieved and cached.");

                        return categories;
                    }
                    else
                    {
                        _logger.LogWarning("The API returned a null categories list.");
                        return Enumerable.Empty<DisplayCategoryViewModel>();
                    }
                }
                else
                {
                    _logger.LogError($"Error fethcing categories from API. Status Code: {response.StatusCode}");
                    throw new CategoryServiceException("Failed to retrieved categories from the API.");
                }
            }
            catch(HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network issue occurred while contacting the Category API.");
                throw new CategoryServiceException("Network error occurred while fetching categories.", httpEx);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GetAllCategories");
                throw new CategoryServiceException("An unexpected error occurred while fetching categories.", ex);
            }
        }

        public async Task<CreateCategoryViewModel> CreateCategory(CreateCategoryViewModel categoryViewModel)
        {
            if (categoryViewModel == null)
            {
                throw new ArgumentNullException(nameof(categoryViewModel));
            }

            try
            {
                _logger.LogInformation("Sending request to create a new category.");

                var response = await _httpClient.PostAsJsonAsync("api/categories", categoryViewModel);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize response into CreateCategoryViewModel
                    var createdCategory = await response.Content.ReadFromJsonAsync<CreateCategoryViewModel>();

                    _cache.Remove(cacheKey);
                    _logger.LogInformation($"Category created successfully with ID: {createdCategory.CategoryId}");

                    return createdCategory;
                }
                else
                {
                    _logger.LogWarning($"Failed to create category: {response.StatusCode}");
                    throw new Exception($"Error while creating category. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the category.");
                throw;  // Re throw exception to be handled at higher level
            }
        }
    }

    public class CategoryServiceException : Exception
    {
        public CategoryServiceException(string message) : base(message) { }

        public CategoryServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
