using IMS_Dashboard.Services.OrderServices.Interface;
using IMS_Dashboard.ViewModels.OrderItemVM;
using IMS_Dashboard.ViewModels.OrderVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Drawing.Text;

namespace IMS_Dashboard.Controllers
{
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("Fetching all Orders");

                var orders = await _orderService.GetAllOrdersWithOrderItems();
                return View(orders);
            }   
            catch(HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching orders from the API.");
                throw;
            }
        }
    }
}
