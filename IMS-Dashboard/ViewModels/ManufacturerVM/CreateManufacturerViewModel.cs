using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.ManufacturerVM
{
    public class CreateManufacturerViewModel
    {
        public Guid ManufacturerId { get; set; }

        [Required(ErrorMessage = "Manufacturer Name is requried.")]
        [Display(Name = "Manufacturer Name")]
        public string ManufacturerName { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [Display(Name = "Contact Number")]
        public string? ContactInfo { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}
