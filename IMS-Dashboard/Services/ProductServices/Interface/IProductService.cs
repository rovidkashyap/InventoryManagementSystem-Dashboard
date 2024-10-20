using IMS_Dashboard.ViewModels.ProductsVM;

namespace IMS_Dashboard.Services.ProductServices.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<DisplayProductViewModel>> GetAllProducts();
        Task<int> GetAllProductsCount();
        Task<bool> CreateProduct(CreateProductViewModel productViewModel);
    }
}
