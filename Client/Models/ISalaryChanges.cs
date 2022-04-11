using Newtonsoft.Json;

namespace Cheddar.Client.Models {
    public class ISalaryChanges {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty("partitionKey")]
        public int UserId { get; set; }
        public DateOnly SalaryChangeDate { get; set; }
        public double SalaryChangeAmount { get; set; }
    }
}