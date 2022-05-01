using Cheddar.Shared.Models;
using Cheddar.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.Services {
    public class BudgetSettingsService {

        private readonly HttpClient ApiClient;
        public BudgetSettingsViewModel budgetSettingsViewModel;
        public IBudgetSettingsModel? userBudgetSettings;

        public BudgetSettingsService(HttpClient apiClient, BudgetSettingsViewModel bsViewModel)
        {
            ApiClient = apiClient;
            budgetSettingsViewModel = bsViewModel;
        }
        public async Task GetMonthlyIncome() {
            budgetSettingsViewModel.budgetSettingsModel = await ApiClient.GetFromJsonAsync<IBudgetSettingsModel>("api/GetMonthlyIncome?");
        }

        public async Task UpsertBudgetSettings(IBudgetSettingsModel budgetSettings, NavigationManager nvm) {  
            await ApiClient.PostAsJsonAsync("api/UpdateBudgetSettings", budgetSettings);
            nvm.NavigateTo("/budget");
        }
    }
}
