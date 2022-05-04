using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class BudgetSettingsModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty(PropertyName = "userId")]
        public int userId { get; set; }
        public double MonthlyIncome { get; set; }
        public int MonthlyBudgetDay { get; set; }
    }
}