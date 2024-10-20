using IMS_Dashboard.ViewModels.ManufacturerVM;

namespace IMS_Dashboard.Services.ManufacturerServices.Interface
{
    public interface IManufacturerService
    {
        Task<IEnumerable<DisplayManufacturerViewModel>> GetAllManufacturers();
        Task<CreateManufacturerViewModel> CreateManufacturer(CreateManufacturerViewModel manufacturerViewModel);
    }
}
