using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class PaymentMethodsModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        public int UserId { get; set; }
        public string? Name { get; set; }
    }
}