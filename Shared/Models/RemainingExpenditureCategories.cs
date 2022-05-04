using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class RemainingExpenditureCategories {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty("partitionKey")]
        public int UserId { get; set; }
        public string? CategoryName { get; set; }
        public int Percentage { get; set; }
        public double Amount { get; set; }
    }
}