using Newtonsoft.Json;

namespace Cheddar.Client.Models {
    public class IBudgetCategoriesModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        public string? name { get; set; }
    }
}