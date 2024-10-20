using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.ProductsVM
{
    public class CreateProductViewModel
    {
        public Guid ProductId { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Display(Name = "Product Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a manufacturer")]
        [Display(Name = "Manufacturer Name")]
        public Guid ManufacturerId { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Product Category")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Please select a unit of measure")]
        [Display(Name = "Product Measure Unit")]
        public Guid UnitOfMeasureId { get; set; }

        [Display(Name = "Product Weight")]
        public decimal? Weight { get; set; }

        [Display(Name = "Product Dimensions")]
        public string Dimensions { get; set; }

        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }

        [Display(Name = "Is Product Active")]
        public bool IsActive { get; set; }

        public IFormFile? ImageFile { get; set; }   // For Handling File Upload

        [Display(Name = "Product Image")]
        public string? ImagePath { get; set; }      // TO Store the path of the uploaded image

        [Display(Name = "Product Description")]
        public string Description { get; set; }

        public List<SelectListItem>? Categories { get; set; }

        public List<SelectListItem>? Manufacturers { get; set; }

        public List<SelectListItem>? UnitOfMeasures { get; set; }
    }
}
