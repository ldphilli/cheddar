using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class IPaymentMethodsModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        public int UserId { get; set; }
        public string? Name { get; set; }
    }
}