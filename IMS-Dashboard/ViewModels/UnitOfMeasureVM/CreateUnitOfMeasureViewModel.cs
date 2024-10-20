using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.UnitOfMeasureVM
{
    public class CreateUnitOfMeasureViewModel
    {
        public Guid UnitOfMeasureId { get; set; }

        [Required(ErrorMessage = "Unit Name is required.")]
        [Display(Name = "Unit Name")]
        public string UnitName { get; set; }

        [Required(ErrorMessage = "Abbreviation is required.")]
        [Display(Name = "Abbreviation")]
        public string? Abbreviation { get; set; }
    }
}
