using IMS_Dashboard.ViewModels.OrderItemVM;

namespace IMS_Dashboard.ViewModels.OrderVM
{
    public class CreateOrderViewModel
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string ShippingAddress { get; set; }
        public string? Notes { get; set; }
        public List<CreateOrderItemViewModel> OrderItems { get; set; } = new List<CreateOrderItemViewModel>();
    }
}
