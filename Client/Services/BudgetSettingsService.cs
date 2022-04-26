using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.Services {
    public class BudgetSettingsService {

        private readonly HttpClient ApiClient;
        public IBudgetSettingsModel userBudgetSettings;

        public BudgetSettingsService(HttpClient apiClient)
        {
            ApiClient = apiClient;
        }
        public async Task GetMonthlyIncome() {
            userBudgetSettings = await ApiClient.GetFromJsonAsync<IBudgetSettingsModel>("api/GetMonthlyIncome?");
        }
    }
}
