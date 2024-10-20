namespace IMS_Dashboard.ViewModels.OrderItemVM
{
    public class DisplayOrderItemViewModel
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
