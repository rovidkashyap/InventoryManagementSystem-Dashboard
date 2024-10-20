using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.SuppliersVM
{
    public class DisplaySupplierViewModel
    {
        public Guid SupplierId { get; set; }
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
