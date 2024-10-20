using IMS_Dashboard.Services.OrderStatusServices.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Dashboard.Controllers
{
    public class OrderStatusController : Controller
    {
        private readonly IOrderStatusService _orderStatusService;
        private readonly ILogger<OrderStatusController> _logger;
        public OrderStatusController(IOrderStatusService orderStatusService, ILogger<OrderStatusController> logger)
        {
            _orderStatusService = orderStatusService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderStatus()
        {
            try
            {
                _logger.LogInformation("Fetching all OrderStatus");

                var orderStatus = await _orderStatusService.GetAllOrderStatus();
                return View(orderStatus);
            }
            catch(HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching orderstatus from the API.");
                throw;
            }
        }
    }
}
