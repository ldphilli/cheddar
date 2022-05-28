using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class RemainingExpenditureCategoriesModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty(PropertyName = "UserId")]
        public string? UserId { get; set; }
        public string? CategoryName { get; set; }
        public int Percentage { get; set; }
        //public double Amount { get; set; }
    }
}