using Cheddar.Client.Models;

namespace Cheddar.Client.ViewModels {
    public class BudgetLineItemVM {
        public BudgetLineItemModel bliModel { get; set; }
        public BudgetLineItemVM()
        {
            bliModel = new BudgetLineItemModel();
            bliModel.Id = Guid.NewGuid().ToString();
            bliModel.UserId = 2;
        }
    }
}