using Cheddar.Client.Models;
using Microsoft.JSInterop;

namespace Cheddar.Client.ViewModels {
    public class BudgetLineItemVM {

        BudgetLineItemModel blm = new BudgetLineItemModel();
        public string BudgetLineName { get; set; }

        public BudgetLineItemVM(BudgetLineItemModel bliModel)
        {
            this.BudgetLineName = bliModel.BudgetLineName;
            Console.WriteLine(blm.BudgetLineName);       
        }
    }
}