using IMS_Dashboard.Services.CustomerServices.Interface;
using IMS_Dashboard.Services.InventoryServices.Interface;
using IMS_Dashboard.Services.OrderServices.Interface;
using IMS_Dashboard.Services.ProductServices.Interface;
using IMS_Dashboard.Services.SupplierServices.Interface;
using IMS_Dashboard.ViewModels.CustomersVM;
using IMS_Dashboard.ViewModels.DashboardVM;
using IMS_Dashboard.ViewModels.InventoryVM;
using IMS_Dashboard.ViewModels.OrderVM;
using IMS_Dashboard.ViewModels.SuppliersVM;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly IInventoryService _inventoryService;
        private readonly IOrderService _orderService;

        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, ICustomerService customerService, 
            IProductService productService, ISupplierService supplierService, IInventoryService inventoryService,
            IOrderService orderService)
        {
            _customerService = customerService;
            _productService = productService;
            _supplierService = supplierService;
            _inventoryService = inventoryService;
            _orderService = orderService;

            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var customerCount = await _customerService.GetAllCustomersCountAsync();
                var productCount = await _productService.GetAllProductsCount();
                var supplierCount = await _supplierService.GetAllSuppliersCountAsync();
                var inventoryCount = await _inventoryService.GetAllInventoryCount();
                var orderCount = await _orderService.GetAllOrdersCount();
                var pendingOrderCount = await _orderService.GetOrdersCountByOrderStatus("Pending");
                var completedOrderCount = await _orderService.GetOrdersCountByOrderStatus("Completed");
                var delieverdOrderCount = await _orderService.GetOrdersCountByOrderStatus("Delivered");

                var top5customer = await _customerService.GetAllCustomers();
                var top5supplier = await _supplierService.GetAllSuppliers();
                var recentOrders = await _orderService.GetRecentOrders(5);

                var recentInventories = await _inventoryService.GetRecentInventories();

                // Create ViewModel
                var dashboardViewModel = new DashboardViewModel
                {
                    CustomerCount = customerCount,
                    ProductCount = productCount,
                    SupplierCount = supplierCount,
                    InventoryCount = inventoryCount,
                    OrderCount = orderCount,
                    PendingOrderCount = pendingOrderCount,
                    CompletedOrderCount = completedOrderCount,
                    DeliveredOrderCount = delieverdOrderCount,
                    TotalOrderCount = pendingOrderCount + completedOrderCount + delieverdOrderCount,

                    top5Customers = top5customer,
                    top5Suppliers = top5supplier,
                    recentOrders = recentOrders,
                    recentInventory = recentInventories,
                };

                // Pass the ViewModel to the View
                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                var dashboard = new DashboardViewModel
                {
                    CustomerCount = 0,
                    ProductCount = 0,
                    SupplierCount = 0,
                    InventoryCount = 0,
                    OrderCount = 0,
                    PendingOrderCount = 0,
                    CompletedOrderCount = 0,
                    DeliveredOrderCount = 0,
                    TotalOrderCount = 0,
                    top5Customers = Enumerable.Empty<DisplayCustomerViewModel>(),
                    top5Suppliers = Enumerable.Empty<DisplaySupplierViewModel>(),
                    recentOrders = Enumerable.Empty<DisplayRecentOrdersViewModel>(),
                    recentInventory = Enumerable.Empty<DisplayRecentInventoryViewModel>()
                };

                return View(dashboard);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult SignalR()
        {
            return View();
        }
    }
}
