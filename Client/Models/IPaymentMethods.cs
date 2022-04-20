using Newtonsoft.Json;

namespace Cheddar.Client.Models {
    public class IPaymentMethodsModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        public string? name { get; set; }
    }
}