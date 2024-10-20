namespace IMS_Dashboard.ViewModels.OrderItemVM
{
    public class CreateOrderItemViewModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
