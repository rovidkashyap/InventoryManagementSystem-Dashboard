using IMS_Dashboard.Services.CategoryServices.Interface;
using IMS_Dashboard.ViewModels.CategoryVM;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;
        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Fetching all Categories");

                var categories = await _categoryService.GetAllCategories();
                return View(categories);
            }
            catch (HttpRequestException ex)
            {
                // Log Exception
                _logger.LogError(ex, "Error fetching categories from the API.");
                throw;
            }
        }

        [HttpGet("CreateCategory")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost("CreateCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for create category.");
                return View(categoryViewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to create a new category.");
                var createdCategory = await _categoryService.CreateCategory(categoryViewModel);

                if (createdCategory != null)
                {
                    
                    _logger.LogInformation("Category creation successsfull. Redirecting to GetAllCategories.");
                    return RedirectToAction(nameof(GetAllCategories));
                }
                else
                {
                    _logger.LogWarning("Category creation failed.");
                    ModelState.AddModelError("", "An error occurred while creating category.");
                    return View(categoryViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category.");
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
