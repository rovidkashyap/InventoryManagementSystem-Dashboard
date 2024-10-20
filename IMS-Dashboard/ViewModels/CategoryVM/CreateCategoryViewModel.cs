using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.CategoryVM
{
    public class CreateCategoryViewModel
    {
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
