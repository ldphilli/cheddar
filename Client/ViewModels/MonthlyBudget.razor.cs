using Cheddar.Shared.Models;
using Cheddar.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class MonthlyBudgetViewModel {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;
        public MonthlyBudgetModel monthlyBudgetModel { get; set; }
        public MonthlyBudgetService monthlyBudgetService;
        private readonly NavigationManager nvm;

        public MonthlyBudgetViewModel(HttpClient apiClient, NavigationManager navManager, ApplicationState applicationState, MonthlyBudgetService mbService)
        {
            ApiClient = apiClient;
            nvm = navManager;
            appState = applicationState;
            monthlyBudgetService = mbService;
        }

         public async Task GetLatestMonthlyBudgetForUser() {
            
            DateTime today = DateTime.Now;
            int month = today.Month;
            int year = today.Year;
            monthlyBudgetModel = await monthlyBudgetService.GetMonthlyBudget(month, year);
         }
    }
}