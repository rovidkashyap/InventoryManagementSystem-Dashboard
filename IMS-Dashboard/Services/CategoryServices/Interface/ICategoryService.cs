using IMS_Dashboard.ViewModels.CategoryVM;

namespace IMS_Dashboard.Services.CategoryServices.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<DisplayCategoryViewModel>> GetAllCategories();
        Task<CreateCategoryViewModel> CreateCategory(CreateCategoryViewModel categoryViewModel);
    }
}
