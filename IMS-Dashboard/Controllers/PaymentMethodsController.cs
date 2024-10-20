using IMS_Dashboard.Services.PaymentMethodServices.Interface;
using IMS_Dashboard.ViewModels.PaymentMethodsVM;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class PaymentMethodsController : Controller
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly ILogger<PaymentMethodsController> _logger;
        public PaymentMethodsController(IPaymentMethodService paymentMethodService, ILogger<PaymentMethodsController> logger)
        {
            _paymentMethodService = paymentMethodService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentMethods()
        {
            try
            {
                _logger.LogInformation("Fetching all PaymentMethods");

                var paymentMethod = await _paymentMethodService.GetAllPaymentMethods();
                return View(paymentMethod);
            }
            catch (HttpRequestException ex)
            {
                // Log Exception
                _logger.LogError(ex, "Error fetching paymentMethods from the API.");
                throw;
            }
        }

        [HttpGet("CreatePaymentMethod")]
        public IActionResult CreatePaymentMethod()
        {
            return View();
        }

        [HttpPost("CreatePaymentMethod")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePaymentMethod(CreatePaymentMethodViewModel paymentViewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for create payment method.");
                return View(paymentViewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to create a new payment method.");
                var createdPaymentMethod = await _paymentMethodService.CreatePaymentMethod(paymentViewModel);

                if (createdPaymentMethod != null)
                {
                    _logger.LogInformation("Payment Method creation successsfull. Redirecting to GetAllPaymentMethods.");
                    return RedirectToAction(nameof(GetAllPaymentMethods));
                }
                else
                {
                    _logger.LogWarning("Payment Method creation failed.");
                    ModelState.AddModelError("", "An error occurred while creating payment method.");
                    return View(paymentViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating payment method.");
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
