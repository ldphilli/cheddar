using Cheddar.Shared.Models;
using Cheddar.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.Services {
    public class MonthlyBudgetService {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;

        public MonthlyBudgetService(HttpClient apiClient, ApplicationState applicationState)
        {
            ApiClient = apiClient;
            appState = applicationState;
        }

        public async Task<MonthlyBudgetModel> GetMonthlyBudget(int month, int year) {
            Console.WriteLine("Entered into monthly budget service GetMonthlyBudget");
            string request = String.Concat("api/GetMonthlyIncome?claim=", appState.Token);
            return await ApiClient.GetFromJsonAsync<MonthlyBudgetModel>(request);
        }
    }
}