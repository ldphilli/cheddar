using System.Text.Json;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Cheddar.Client.Models {
    public class BudgetLineItemModel {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string BLIid { get; set; }
        public string BudgetLineName { get; set; }
        public double Cost { get; set; }
        public string Category { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? ContractEndDate { get; set; }
    }
}