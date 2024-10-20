using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.PaymentMethodsVM
{
    public class DisplayPaymentMethodViewModel
    {
        [Display(Name = "Payment Method Name")]
        public string PaymentMethodName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
