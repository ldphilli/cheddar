using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetVM {

        private readonly HttpClient ApiClient;

        public BudgetVM(HttpClient apiClient)
        {
            ApiClient = apiClient;
        }

        public List<BudgetLineItemModel>? budgetLineItems = new List<BudgetLineItemModel>();
        public List<string>? budgetCategories = new List<string>();

        public Dictionary<string, double> CostPerCategory { get; set; }

        public double TotalCost { get; set; }

        /// <summary>
        /// Add BudgetLineItem items to the container
        /// </summary>
        public async Task AddItemsToContainerAsync(BudgetLineItemModel budgetLineItem, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/CreateBudgetLineItem", budgetLineItem);
            nvm.NavigateTo("/budget");
        }

        public async Task GetBudgetLineItems() {

            budgetLineItems = await ApiClient.GetFromJsonAsync<List<BudgetLineItemModel>>("api/GetBudgetLineItems?");
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
