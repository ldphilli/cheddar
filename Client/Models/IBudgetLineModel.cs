using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cheddar.Client.Models {
    public class BudgetLineItemModel {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public string BLIid { get; set; }
        public string BudgetLineName { get; set; }
        public double Cost { get; set; }
        public string Category { get; set; }
        public string PaymentMethod { get; set; }
        public string ContractEndDate { get; set; }
        
        /*public override string ToString() {
            return JsonSerializer.Serialize(this);
        }*/

        /*public BudgetLineItemModel() {
            this.BudgetLineName = BudgetLineName;
        }*/
    }
}