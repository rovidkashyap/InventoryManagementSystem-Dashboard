using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.TransactionTypeVM
{
    public class CreateTransactionTypeViewModel
    {
        public Guid TransactionTypeId { get; set; }

        [Required(ErrorMessage = "Transaction type is required.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
