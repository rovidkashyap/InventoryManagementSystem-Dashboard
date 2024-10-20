using IMS_Dashboard.ViewModels.CategoryVM;
using IMS_Dashboard.ViewModels.OrderVM;

namespace IMS_Dashboard.Services.OrderServices.Interface
{
    public interface IOrderService
    {
        Task<int> GetAllOrdersCount();
        Task<IEnumerable<DisplayOrdersWithOrderItemsViewModel>> GetAllOrdersWithOrderItems();
        Task<int> GetOrdersCountByOrderStatus(string orderStatus);

        Task<IEnumerable<DisplayRecentOrdersViewModel>> GetRecentOrders(int count);
    }
}
