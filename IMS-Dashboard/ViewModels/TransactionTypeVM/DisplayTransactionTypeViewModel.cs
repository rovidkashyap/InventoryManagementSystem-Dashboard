using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.TransactionTypeVM
{
    public class DisplayTransactionTypeViewModel
    {
        public Guid TransactionTypeId { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
