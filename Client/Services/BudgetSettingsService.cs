using Cheddar.Shared.Models;
using Cheddar.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.Services {
    public class BudgetSettingsService {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;

        public BudgetSettingsService(HttpClient apiClient, ApplicationState applicationState)
        {
            ApiClient = apiClient;
            appState = applicationState;
        }
        public async Task<BudgetSettingsModel> GetMonthlyIncome() {
            Console.WriteLine("Entered into budget settings service GetMonthlyIncome");
            string request = String.Concat("api/GetMonthlyIncome?claim=", appState.Token);
            return await ApiClient.GetFromJsonAsync<BudgetSettingsModel>(request);
        }

        public async Task CreateOrUpdateBudgetSettingsDoc(BudgetSettingsModel budgetSettings) {  
            string request = String.Concat("api/CreateOrUpdateBudgetSettingsDoc?claim=", appState.Token);
            await ApiClient.PostAsJsonAsync(request, budgetSettings);
        }

        public async Task CreateRemainingExpenditureCategoriesDoc(RemainingExpenditureCategoriesModel remainingExpenditureCategory) {

            string request = String.Concat("api/CreateRemainingExpenditureCategories?claim=", appState.Token);
            await ApiClient.PostAsJsonAsync(request, remainingExpenditureCategory);
        }
    }
}
