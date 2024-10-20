using IMS_Dashboard.Services.CategoryServices.Interface;
using IMS_Dashboard.Services.ManufacturerServices.Interface;
using IMS_Dashboard.Services.ProductServices.Interface;
using IMS_Dashboard.Services.UnitOfMeasureServices.Interface;
using IMS_Dashboard.ViewModels.ProductsVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IUnitOfMeasureService _unitOfMeasureService;

        private readonly ILogger<ProductsController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ProductsController(IWebHostEnvironment hostingEnvironment, IProductService productService, ICategoryService categoryService, 
            IManufacturerService manufacturerService, IUnitOfMeasureService unitOfMeasureService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _unitOfMeasureService = unitOfMeasureService;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("Fetching all Products");

                var products = await _productService.GetAllProducts();
                return View(products);
            }
            catch(HttpRequestException ex)
            {
                // Log Exception
                _logger.LogError(ex, "Error fetching products from the API.");
                throw;
            }
        }

        [HttpGet("CreateProduct")]
        public async Task<IActionResult> CreateProduct()
        {
            var createProductViewModel = new CreateProductViewModel();

            // Fetch Category from the API
            await PopulateDropdowns(createProductViewModel);
            return View(createProductViewModel);
        }

        [HttpPost("CreateProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel productViewModel)
        {

            if (!ModelState.IsValid)
            {
                // Log validation error
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    _logger.LogError($"Model validation error: {error.ErrorMessage}");
                    _logger.LogInformation($"CategoryId: {productViewModel.CategoryId}");
                    _logger.LogInformation($"ManufacturerId: {productViewModel.ManufacturerId}");
                    _logger.LogInformation($"UnitOfMeasureId: {productViewModel.UnitOfMeasureId}");
                }

                // Fetch Categories, Maufacturers, and UnitOfMeasures again in case of validation
                await PopulateDropdowns(productViewModel);
                return View(productViewModel);
            }

            try
            {
                if(productViewModel.ImageFile != null && productViewModel.ImageFile.Length > 0)
                {
                    // Define the path to save the image
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(productViewModel.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productViewModel.ImageFile.CopyToAsync(stream);
                    }

                    // Verify the file exists after saving
                    if(System.IO.File.Exists(filePath))
                    {
                        _logger.LogInformation($"File successfully saved: {fileName}");
                        productViewModel.ImagePath = $"/images/{fileName}";
                    }
                    else
                    {
                        _logger.LogWarning($"File not found after saving: {fileName}");
                        ModelState.AddModelError("", "An error occurred while saving the image.");
                        await PopulateDropdowns(productViewModel);
                        return View(productViewModel);
                    }

                    // Save the file path in the view model
                    productViewModel.ImagePath = $"/images/{fileName}";
                }
                else
                {
                    _logger.LogWarning("No Image file was uploaded.");
                }

                _logger.LogInformation("Attempting to create a new product.");
                bool result = await _productService.CreateProduct(productViewModel);

                if (result)
                {
                    _logger.LogInformation("Product creation successful. Redirecting to GetAllProducts");
                    return RedirectToAction(nameof(GetAllProducts));
                }
                else
                {
                    _logger.LogWarning("Product creation failed.");
                    ModelState.AddModelError("", "An error occurred while creating the product.");
                    await PopulateDropdowns(productViewModel);
                    return View(productViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product.");
                ModelState.AddModelError("", "An error occurred while creating the product."); // User-friendly error message
                await PopulateDropdowns(productViewModel);
                return View(productViewModel); // Return the view with the error message
            }
        }

        private async Task PopulateDropdowns(CreateProductViewModel productViewModel)
        {
            var categories = await _categoryService.GetAllCategories();
            productViewModel.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            var manufacturers = await _manufacturerService.GetAllManufacturers();
            productViewModel.Manufacturers = manufacturers.Select(m => new SelectListItem
            {
                Value = m.ManufacturerId.ToString(),
                Text = m.ManufacturerName
            }).ToList();

            var unitOfMeasures = await _unitOfMeasureService.GetAllUnitofMeasures();
            productViewModel.UnitOfMeasures = unitOfMeasures.Select(u => new SelectListItem
            {
                Value = u.UnitOfMeasureId.ToString(),
                Text = u.UnitName
            }).ToList();
        }
    }
}
