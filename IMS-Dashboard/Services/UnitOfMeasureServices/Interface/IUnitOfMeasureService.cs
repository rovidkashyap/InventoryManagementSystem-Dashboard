using IMS_Dashboard.ViewModels.UnitOfMeasureVM;

namespace IMS_Dashboard.Services.UnitOfMeasureServices.Interface
{
    public interface IUnitOfMeasureService
    {
        Task<IEnumerable<DisplayUnitViewModel>> GetAllUnitofMeasures();
        Task<CreateUnitOfMeasureViewModel> CreateUnit(CreateUnitOfMeasureViewModel unitViewModel);
    }
}
