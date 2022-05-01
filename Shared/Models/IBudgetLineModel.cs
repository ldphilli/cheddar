using Newtonsoft.Json;

namespace Cheddar.Shared.Models {
    public class BudgetLineItemModel {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty(PropertyName = "UserId")]
        public int UserId { get; set; }
        public string? BudgetLineName { get; set; }
        public double Cost { get; set; }
        public string? Category { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? ContractEndDate { get; set; }
    }
}