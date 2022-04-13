using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class IBudgetCategoriesModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        public string? name { get; set; }
    }
}