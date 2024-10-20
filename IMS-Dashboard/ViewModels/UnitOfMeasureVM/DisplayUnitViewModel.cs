using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.UnitOfMeasureVM
{
    public class DisplayUnitViewModel
    {
        public Guid UnitOfMeasureId { get; set; }

        [Display(Name = "Unit Name")]
        public string UnitName { get; set; }

        [Display(Name = "Abbreviation")]
        public string? Abbreviation { get; set; }
    }
}
