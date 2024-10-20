using System.Globalization;

namespace IMS_Dashboard.ViewModels.InventoryVM
{
    public class DisplayRecentInventoryViewModel
    {
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public int QuantityAdded { get; set; }
        public int QuantityRemoved { get; set; }
        public int QuantityInStock { get; set; }
        public string TransactionName { get; set; }
        public DateTime TransactionDate { get; set; }
        //public DateTime TransactionDate 
        //{ 
        //    get 
        //    {
        //        return DateTime.ParseExact(TransactionDateString, "yyyy-dd-MMM", CultureInfo.InvariantCulture);
        //    } 
        //}
    }
}
