using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;
using Cheddar.Client.Services;

namespace Cheddar.Client.ViewModels {
    public class BudgetViewModel {

        private readonly HttpClient ApiClient;
        private readonly IHttpClientFactory _factory;
        private readonly ApplicationState appState;
        //private static readonly Lazy<ApplicationState> appState = new Lazy<ApplicationState>(() => new ApplicationState());
        private readonly NavigationManager nvm;
        public IBudgetSettingsService budgetSettingsService;

        public BudgetViewModel(HttpClient apiClient, IHttpClientFactory factory, NavigationManager navManager, ApplicationState applicationState, IBudgetSettingsService bsService)
        {
            ApiClient = apiClient;
            _factory = factory;
            nvm = navManager;
            appState = applicationState;
            budgetSettingsService = bsService;
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
            
            try {
                budgetLineItems = await ApiClient.GetFromJsonAsync<List<BudgetLineItemModel>>("api/GetBudgetLineItems");
                CalculateExpenditureByCategories();
            }
            catch (AccessTokenNotAvailableException exception) {
                exception.Redirect();
            }
        }

        public async Task GetBudgetSettingsForUser() {
            Console.WriteLine("Entered into Budget GetBudgetSettingsForUser");
            appState.budgetSettingsModel = await budgetSettingsService.GetMonthlyIncome();
            CalculateExpenditureByCategories();
        }

        public Dictionary<string, double> CalculateExpenditureByCategories() {

            var budget = appState.monthlyBudgetModel.Outgoing;
            TotalCost = budgetLineItems.Sum(x => x.Cost);
            CostPerCategory = new Dictionary<string, double>(budgetLineItems
                .GroupBy(x => x.Category.Name)
                .Select(grouping => new KeyValuePair<string, double>(grouping.Key, Math.Round((grouping.Sum(x => x.Cost) / budget) * 100, 2))));
                
            //Add remaining as final category and calculate the remaining amount
            CostPerCategory.Add("Remaining", Math.Round(((budget - TotalCost) / budget) * 100, 2));
            
            return CostPerCategory;
        }
    }
}
