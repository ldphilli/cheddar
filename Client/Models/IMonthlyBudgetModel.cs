using Newtonsoft.Json;

namespace Cheddar.Client.Models {
    public class IMonthlyBudgetModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty("partitionKey")]
        public int UserId { get; set; }
        public double Income { get; set; }
        public double Outgoing { get; set; }
        public double Remaining { get; set; }
    }
}