using IMS_Dashboard.Services.ManufacturerServices.Interface;
using IMS_Dashboard.ViewModels.ManufacturerVM;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class ManufacturersController : Controller
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly ILogger<ManufacturersController> _logger;
        public ManufacturersController(IManufacturerService manufacturerService, ILogger<ManufacturersController> logger)
        {
            _manufacturerService = manufacturerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllManufacturers()
        {
            try
            {
                _logger.LogInformation("Fetching all Manufacturers");

                var manufacturer = await _manufacturerService.GetAllManufacturers();
                return View(manufacturer);
            }
            catch (HttpRequestException ex)
            {
                // Log Exception
                _logger.LogError(ex, "Error fetching manufacturers from the API.");
                throw;
            }
        }

        [HttpGet("CreateManufacturer")]
        public IActionResult CreateManufacturer()
        {
            return View();
        }

        [HttpPost("CreateManufacturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateManufacturer(CreateManufacturerViewModel manufacturerViewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for create manufacturer.");
                return View(manufacturerViewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to create a new supplier.");
                var createdManufacturer = await _manufacturerService.CreateManufacturer(manufacturerViewModel);

                if (createdManufacturer != null)
                {
                    _logger.LogInformation("Manufacturers creation successsfull. Redirecting to GetAllManufacturers.");
                    return RedirectToAction(nameof(GetAllManufacturers));
                }
                else
                {
                    _logger.LogWarning("Manufacturer creation failed.");
                    ModelState.AddModelError("", "An error occurred while creating manufacturer.");
                    return View(manufacturerViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating manufacturer.");
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
