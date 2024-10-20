using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.PaymentMethodsVM
{
    public class CreatePaymentMethodViewModel
    {
        public Guid PaymentMethodId { get; set; }

        [Required(ErrorMessage = "Payment Method Name is required.")]
        [Display(Name = "Payment Method Name")]
        public string PaymentMethodName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Active Status is required.")]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
