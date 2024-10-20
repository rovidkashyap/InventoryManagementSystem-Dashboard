using IMS_Dashboard.ViewModels.OrderStatusVM;

namespace IMS_Dashboard.Services.OrderStatusServices.Interface
{
    public interface IOrderStatusService
    {
        Task<IEnumerable<DisplayOrderStatusViewModel>> GetAllOrderStatus();
    }
}
