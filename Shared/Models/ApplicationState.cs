//Put in shared things in here like MonthlyIncome
using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class ApplicationState {
        public BudgetSettingsModel budgetSettingsModel = new BudgetSettingsModel();//{ get; set; }

        public string? Token { get; set; }
    }
}