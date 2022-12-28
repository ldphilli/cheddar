//Put in shared things in here like MonthlyIncome
using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class ApplicationState {
        public BudgetSettingsModel budgetSettingsModel = new BudgetSettingsModel();
        public MonthlyBudgetModel monthlyBudgetModel = new MonthlyBudgetModel();
        public BudgetLineItemModel BudgetLineItemModel = new BudgetLineItemModel();
    }
}