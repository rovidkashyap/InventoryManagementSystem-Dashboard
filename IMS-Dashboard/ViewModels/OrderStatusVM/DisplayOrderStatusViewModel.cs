using System.ComponentModel.DataAnnotations;

namespace IMS_Dashboard.ViewModels.OrderStatusVM
{
    public class DisplayOrderStatusViewModel
    {
        [Display(Name = "Order Status")]
        public string OrderStatusName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
