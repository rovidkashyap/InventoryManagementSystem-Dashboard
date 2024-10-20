using IMS_Dashboard.Services.SupplierServices.Interface;
using IMS_Dashboard.ViewModels.SuppliersVM;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SuppliersController> _logger;
        public SuppliersController(ISupplierService supplierService, ILogger<SuppliersController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            try
            {
                _logger.LogInformation("Fetching all Suppliers");

                var suppliers = await _supplierService.GetAllSuppliers();
                return View(suppliers);
            }
            catch (HttpRequestException ex)
            {
                // Log Exception
                _logger.LogError(ex, "Error fetching suppliers from the API.");
                throw;
            }
        }

        [HttpGet("CreateSupplier")]
        public IActionResult CreateSupplier()
        {
            return View();
        }

        [HttpPost("CreateSupplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier(CreateSupplierViewModel supplierViewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for create supplier.");
                return View(supplierViewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to create a new supplier.");
                var createdSupplier = await _supplierService.CreateSupplier(supplierViewModel);

                if(createdSupplier != null)
                {
                    _logger.LogInformation("Spplier creation successsfull. Redirecting to GetAllSuppliers.");
                    return RedirectToAction(nameof(GetAllSuppliers));
                }
                else
                {
                    _logger.LogWarning("Supplier creation failed.");
                    ModelState.AddModelError("", "An error occurred while creating supplier.");
                    return View(supplierViewModel);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating supplier.");
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
