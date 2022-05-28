using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class RemainingExpenditureCategoriesWithAmountModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty(PropertyName = "UserId")]
        public string? UserId { get; set; }
        public string? CategoryName { get; set; }
        public double Amount { get; set; }
    }
}