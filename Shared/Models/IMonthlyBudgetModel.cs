using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class MonthlyBudgetModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty(PropertyName = "UserId")]
        public int UserId { get; set; }
        public double Income { get; set; }
        public double Outgoing { get; set; }
        public double Remaining { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<RemainingExpenditureCategories>? expenditureCategories { get; set; }
    }
}