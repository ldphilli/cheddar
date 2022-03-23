using Cheddar.Client.Models;
using Microsoft.JSInterop;

namespace Cheddar.Client.ViewModels {
    public class BudgetLineItemVM {
        public string BudgetLineName { get; set; }
        public double Cost { get; set; }
        public string Category { get; set; }
        public string PaymentMethod { get; set; }
        public string ContractEndDate { get; set; }
        public BudgetLineItemVM(BudgetLineItemModel bliModel)
        {
            this.BudgetLineName = bliModel.BudgetLineName;
            this.Category = bliModel.Category;
            this.PaymentMethod = bliModel.PaymentMethod;
            this.ContractEndDate = bliModel.ContractEndDate;
        }
    }
}