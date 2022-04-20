using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class IBudgetSettingsModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty("partitionKey")]
        public int UserId { get; set; }
        public double MonthlyIncome { get; set; }
    }
}