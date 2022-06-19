using Cheddar.Shared.Models;
using Cheddar.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Cheddar.Client.Services {
    public class BudgetSettingsService {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;
        private readonly IAccessTokenProvider tp;

        public BudgetSettingsService(HttpClient apiClient, ApplicationState applicationState, IAccessTokenProvider tp)
        {
            ApiClient = apiClient;
            appState = applicationState;

            this.tp = tp;
        }
        public async Task<BudgetSettingsModel?> GetMonthlyIncome() {
            return await ApiClient.GetFromJsonAsync<BudgetSettingsModel>("api/GetMonthlyIncome");
        }

        public async Task CreateOrUpdateBudgetSettingsDoc(BudgetSettingsModel budgetSettings) {  
            await ApiClient.PostAsJsonAsync("api/CreateOrUpdateBudgetSettingsDoc", budgetSettings);
        }

        public async Task CreateRemainingExpenditureCategoriesDoc(RemainingExpenditureCategoriesModel remainingExpenditureCategory) {

            await ApiClient.PostAsJsonAsync("api/CreateRemainingExpenditureCategories", remainingExpenditureCategory);
        }
    }
}
