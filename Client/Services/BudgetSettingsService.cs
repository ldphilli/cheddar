using Cheddar.Shared.Models;
using Cheddar.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.Services {
    public class BudgetSettingsService {

        private readonly HttpClient ApiClient;

        public BudgetSettingsService(HttpClient apiClient)
        {
            ApiClient = apiClient;
        }
        public async Task<BudgetSettingsModel> GetMonthlyIncome() {
            Console.WriteLine("Entered into budget settings service GetMonthlyIncome");
            return await ApiClient.GetFromJsonAsync<BudgetSettingsModel>("api/GetMonthlyIncome?");
        }

        public async Task CreateOrUpdateBudgetSettingsDoc(BudgetSettingsModel budgetSettings) {  
            await ApiClient.PostAsJsonAsync("api/CreateOrUpdateBudgetSettingsDoc", budgetSettings);
        }

        public async Task CreateRemainingExpenditureCategoriesDoc(RemainingExpenditureCategoriesModel remainingExpenditureCategory) {

            await ApiClient.PostAsJsonAsync("api/CreateRemainingExpenditureCategories", remainingExpenditureCategory);
        }
    }
}
