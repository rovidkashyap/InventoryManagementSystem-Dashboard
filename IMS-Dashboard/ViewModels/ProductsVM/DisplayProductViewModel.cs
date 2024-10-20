using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.ProductsVM
{
    public class DisplayProductViewModel
    {
        public Guid ProductId { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Manufacturer")]
        public string ManufacturerName { get; set; }

        [Display(Name = "Product Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Measuring Unit")]
        public string UnitOfMeasureName { get; set; }

        [Display(Name = "Product Weight")]
        public decimal? Weight { get; set; }

        [Display(Name = "Product Dimension")]
        public string Dimensions { get; set; }

        [Display(Name = "Is Product Active")]
        public bool IsActive { get; set; }

    }
}
