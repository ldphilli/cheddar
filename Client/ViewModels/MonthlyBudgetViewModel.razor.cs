using Cheddar.Shared.Models;
using Cheddar.Client.Services;
using Microsoft.AspNetCore.Components;

namespace Cheddar.Client.ViewModels {
    public class MonthlyBudgetViewModel {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;
        public MonthlyBudgetModel monthlyBudgetModel => appState.monthlyBudgetModel;
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
            appState.monthlyBudgetModel = await monthlyBudgetService.GetMonthlyBudget(month, year);
            if(appState.monthlyBudgetModel == null)
            {
                appState.monthlyBudgetModel.Income = 0;
                appState.monthlyBudgetModel.Outgoing = 0;
                appState.monthlyBudgetModel.Remaining = 0;
            }
        }

        public async Task GetAvailableYearsForUser() {

        }

        public async Task GetAvailableMonthsForSelectedYear() {

        }
    }
}