using Cheddar.Client.Models;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetVM {

        public List<BudgetLineItemModel>? budgetLineItems = new List<BudgetLineItemModel>();
        public List<string> budgetCategories = new List<string>();
        /// <summary>
        /// Add BudgetLineItem items to the container
        /// </summary>
        public async Task AddItemsToContainerAsync(BudgetLineItemModel budgetLineItem, NavigationManager nvm) {
            HttpClient client = new HttpClient();
            var url = "http://localhost:7071/api/CreateBudgetLineItem";
            await client.PostAsJsonAsync(url, budgetLineItem);
            nvm.NavigateTo("/budget");
        }

        public async Task GetBudgetLineItems() {

            HttpClient client = new HttpClient();
            var url = "http://localhost:7071/api/GetBudgetLineItems";
            budgetLineItems = await client.GetFromJsonAsync<List<BudgetLineItemModel>>(url);
            //budgetCategories = budgetLineItems.Select(item => item.Category).Distinct();
        }

        public void CalculateExpenditureByCategories() {

        }
    }
}
