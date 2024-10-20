using IMS_Dashboard.ViewModels.CustomersVM;

namespace IMS_Dashboard.Services.CustomerServices.Interface
{
    public interface ICustomerService
    {
        Task<int> GetAllCustomersCountAsync();
        Task<IEnumerable<DisplayCustomerViewModel>> GetAllCustomers();
        Task<CreateCustomerViewModel> CreateCustomerAsync(CreateCustomerViewModel customerViewModel);
    }
}
