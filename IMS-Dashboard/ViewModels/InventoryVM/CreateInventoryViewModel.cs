using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS_Dashboard.ViewModels.InventoryVM
{
    public class CreateInventoryViewModel
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public int QuantityAdded { get; set; }
        public int QuantityRemoved { get; set; }
        public int QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid TransactionTypeId { get; set; }
        public string? Remark { get; set; }
        public DateTime TransactionDate { get; set; }

        public List<SelectListItem>? Products { get; set; }
        public List<SelectListItem>? Suppliers { get; set; }
        public List<SelectListItem>? TransactionTypes { get; set; }
    }
}
