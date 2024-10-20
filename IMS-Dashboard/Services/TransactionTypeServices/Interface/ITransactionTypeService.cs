using IMS_Dashboard.ViewModels.TransactionTypeVM;

namespace IMS_Dashboard.Services.TransactionTypeServices.Interface
{
    public interface ITransactionTypeService
    {
        Task<IEnumerable<DisplayTransactionTypeViewModel>> GetAllTransactionTypes();
        Task<CreateTransactionTypeViewModel> CreateTransactionType(CreateTransactionTypeViewModel transactionViewModel);
    }
}
