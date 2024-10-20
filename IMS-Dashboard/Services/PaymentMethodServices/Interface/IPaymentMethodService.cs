using IMS_Dashboard.ViewModels.PaymentMethodsVM;

namespace IMS_Dashboard.Services.PaymentMethodServices.Interface
{
    public interface IPaymentMethodService
    {
        Task<IEnumerable<DisplayPaymentMethodViewModel>> GetAllPaymentMethods();
        Task<CreatePaymentMethodViewModel> CreatePaymentMethod(CreatePaymentMethodViewModel paymentViewModel);
    }
}
