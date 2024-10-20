using IMS_Dashboard.Services.UnitOfMeasureServices.Interface;
using IMS_Dashboard.ViewModels.UnitOfMeasureVM;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class UnitOfMeasuresController : Controller
    {
        private readonly IUnitOfMeasureService _unitOfMeasuresService;
        private readonly ILogger<UnitOfMeasuresController> _logger;
        public UnitOfMeasuresController(IUnitOfMeasureService unitOfMeasuresService, ILogger<UnitOfMeasuresController> logger)
        {
            _unitOfMeasuresService = unitOfMeasuresService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUnitOfMeasures()
        {
            try
            {
                _logger.LogInformation("Fetching all Units");

                var units = await _unitOfMeasuresService.GetAllUnitofMeasures();
                return View(units);
            }
            catch (HttpRequestException ex)
            {
                // Log Exception
                _logger.LogError(ex, "Error fetching unitsOfMeasures from the API.");
                throw;
            }
        }

        [HttpGet("CreateUnit")]
        public IActionResult CreateUnit()
        {
            return View();
        }

        [HttpPost("CreateUnit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUnit(CreateUnitOfMeasureViewModel unitViewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for create unit of measure");
                return View(unitViewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to create a new units.");
                var createdSupplier = await _unitOfMeasuresService.CreateUnit(unitViewModel);

                if (unitViewModel != null)
                {
                    _logger.LogInformation("Unit of Measures creation successsfull. Redirecting to GetAllUnit.");
                    return RedirectToAction(nameof(GetAllUnitOfMeasures));
                }
                else
                {
                    _logger.LogWarning("Unit Of Measure creation failed.");
                    ModelState.AddModelError("", "An error occurred while creating units.");
                    return View(unitViewModel);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating units.");
                return StatusCode(500, "Internal Server Errror.");
            }
        }
    }
}
