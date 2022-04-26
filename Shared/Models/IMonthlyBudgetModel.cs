using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class IMonthlyBudgetModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        public int UserId { get; set; }
        public double Income { get; set; }
        public double Outgoing { get; set; }
        public double Remaining { get; set; }
    }
}