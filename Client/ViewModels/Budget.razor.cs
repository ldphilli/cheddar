using Cheddar.Client.Models;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetVM {

        public List<BudgetLineItemModel>? budgetLineItems = new List<BudgetLineItemModel>();
        public List<string>? budgetCategories = new List<string>();

        public Dictionary<string, double> CostPerCategory { get; set; }

        public double TotalCost { get; set; }

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
            CalculateExpenditureByCategories();
        }

        public Dictionary<string, double> CalculateExpenditureByCategories() {

            var budget = 1000;
            TotalCost = budgetLineItems.Sum(x => x.Cost);
            CostPerCategory = new Dictionary<string, double>(budgetLineItems
                .GroupBy(x => x.Category)
                .Select(grouping => new KeyValuePair<string, double>(grouping.Key, Math.Round((grouping.Sum(x => x.Cost) / budget) * 100, 2))));
                
            //Add remaining as final category and calculate the remaining amount
            CostPerCategory.Add("Remaining", Math.Round(((budget - TotalCost) / budget) * 100, 2));
            
            return CostPerCategory;
        }
    }
}
