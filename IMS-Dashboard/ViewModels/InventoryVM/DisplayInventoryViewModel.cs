using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.InventoryVM
{
    public class DisplayInventoryViewModel
    {
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Display(Name = "Quantity Added")]
        public int QuantityAdded { get; set; }

        [Display(Name = "Quantity Removed")]
        public int QuantityRemoved { get; set; }

        [Display(Name = "Quantity In Stock")]
        public int QuantityInStock { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Transaction type")]
        public string TransactionTypeName { get; set; }

        [Display(Name = "Remark")]
        public string? Remark { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
    }
}
