using IMS_Dashboard.Services.CustomerServices.Interface;
using IMS_Dashboard.ViewModels.CustomersVM;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;
        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                _logger.LogInformation("Fetching all customers");

                var customers = await _customerService.GetAllCustomers();
                return View(customers);
            }
            catch(HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching customers from the API.");
                throw;
            }
        }

        [HttpGet("CreateCustomer")]
        public IActionResult CreateCustomer()
        {
            return View();  
        }

        [HttpPost("CreateCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(CreateCustomerViewModel customerViewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateCustomer.");
                return View(customerViewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to create a new customer.");
                var createdCustomer = await _customerService.CreateCustomerAsync(customerViewModel);

                if (createdCustomer != null)
                {
                    _logger.LogInformation("Customer creation successful. Redirecting to GetAllCustomers");
                    return RedirectToAction(nameof(GetAllCustomers));
                }
                else
                {
                    _logger.LogWarning("Customer creation failed.");
                    ModelState.AddModelError("", "An error occurred while creating the customer.");
                    return View(customerViewModel);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating customer.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
