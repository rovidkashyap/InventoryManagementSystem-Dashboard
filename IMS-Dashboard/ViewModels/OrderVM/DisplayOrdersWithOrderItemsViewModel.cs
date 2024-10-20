using IMS_Dashboard.ViewModels.OrderItemVM;
using System.Runtime.CompilerServices;

namespace IMS_Dashboard.ViewModels.OrderVM
{
    public class DisplayOrdersWithOrderItemsViewModel
    {
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethodName { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }

        public List<DisplayOrderItemViewModel> OrderItems { get; set; }

    }
}
