using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.ManufacturerVM
{
    public class DisplayManufacturerViewModel
    {
        public Guid ManufacturerId { get; set; }

        [Display(Name = "Manufacturer Name")]
        public string ManufacturerName { get; set; }

        [Display(Name = "Contact Number")]
        public string? ContactInfo { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}
