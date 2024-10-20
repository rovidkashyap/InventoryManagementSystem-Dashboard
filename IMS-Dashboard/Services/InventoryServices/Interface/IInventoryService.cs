using IMS_Dashboard.ViewModels.InventoryVM;

namespace IMS_Dashboard.Services.InventoryServices.Interface
{
    public interface IInventoryService
    {
        Task<int> GetAllInventoryCount();
        Task<IEnumerable<DisplayInventoryViewModel>> GetAllInventories();
        Task<IEnumerable<DisplayRecentInventoryViewModel>> GetRecentInventories();
        Task<bool> CreateInventory(CreateInventoryViewModel inventoryViewModel);
    }
}
