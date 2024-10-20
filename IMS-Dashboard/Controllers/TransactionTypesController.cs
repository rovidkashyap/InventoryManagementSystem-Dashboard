using IMS_Dashboard.Services.TransactionTypeServices.Interface;
using IMS_Dashboard.ViewModels.TransactionTypeVM;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class TransactionTypesController : Controller
    {
        private readonly ITransactionTypeService _transactionTypeService;
        private readonly ILogger<TransactionTypesController> _logger;
        public TransactionTypesController(ITransactionTypeService transactionTypeServices, ILogger<TransactionTypesController> logger)
        {
            _transactionTypeService = transactionTypeServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionTypes()
        {
            try
            {
                _logger.LogInformation("Fetching all Transaction Types.");

                var transactionTypes = await _transactionTypeService.GetAllTransactionTypes();
                return View(transactionTypes);
            }
            catch (HttpRequestException ex)
            {
                // Log Exception
                _logger.LogError(ex, "Error fetching transactionTypes from the API.");
                throw;
            }
        }

        [HttpGet("CreateTransactionType")]
        public IActionResult CreateTransactionType()
        {
            return View();
        }

        [HttpPost("CreateTransactionType")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransactionType(CreateTransactionTypeViewModel transactionViewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for create transaction type.");
                return View(transactionViewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to create a new transaction type.");
                var createdTransaction = await _transactionTypeService.CreateTransactionType(transactionViewModel);

                if (createdTransaction != null)
                {
                    _logger.LogInformation("Transaction Type creation successsfull. Redirecting to GetAllTransactionTypes.");
                    return RedirectToAction(nameof(GetAllTransactionTypes));
                }
                else
                {
                    _logger.LogWarning("Transaction Type creation failed.");
                    ModelState.AddModelError("", "An error occurred while creating transaction type.");
                    return View(transactionViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating transaction Type.");
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
