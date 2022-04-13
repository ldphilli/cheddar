using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class ISalaryUpdateModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        //[JsonProperty("partitionKey")]
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
    }
}