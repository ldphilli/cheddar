using System.Net.Http.Json;
//using Newtonsoft.Json;
using Cheddar.Client.Models;

namespace Cheddar.Client.ViewModels {
    public class BudgetVM {

        /// <summary>
        /// Add BudgetLineItem items to the container
        /// </summary>
        public async Task AddItemsToContainerAsync(BudgetLineItemModel budgetLineItem) {
            HttpClient client = new HttpClient();
            var url = "http://localhost:7071/api/CreateBudgetLineItem";
            await client.PostAsJsonAsync(url, budgetLineItem);
        }
    }
}

