using Cheddar.Shared.Models;
using Cheddar.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Cheddar.Client.ViewModels {
    public class MonthlyBudgetViewModel {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;
        public MonthlyBudgetModel monthlyBudgetModel => appState.monthlyBudgetModel;
        public MonthlyBudgetService monthlyBudgetService;
        private readonly NavigationManager nvm;
        public List<int> years = new List<int>();
        public List<MonthModel> monthsForYear = new List<MonthModel>();
        public int selectedYear;
        public MonthModel selectedMonth;
        public List<MonthlyBudgetModel> allMonthlyBudgetsForUser;

        public MonthlyBudgetViewModel(HttpClient apiClient, NavigationManager navManager, ApplicationState applicationState, MonthlyBudgetService mbService)
        {
            ApiClient = apiClient;
            nvm = navManager;
            appState = applicationState;
            monthlyBudgetService = mbService;
        }

        public async Task SetupLatestMonthlyBudgetForUser() {
            
           await GetAllMonthlyBudgetsForUser();

           GetAvailableYearsForUser();
           selectedYear = years.LastOrDefault();

           GetMonthsForSelectedYear();
           selectedMonth = monthsForYear.LastOrDefault();

           appState.monthlyBudgetModel = allMonthlyBudgetsForUser.OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .FirstOrDefault();
        }

        public async Task ReloadBudgetForSelectedYear() {

           GetMonthsForSelectedYear();
           selectedMonth = monthsForYear.FirstOrDefault();
           appState.monthlyBudgetModel = await monthlyBudgetService.GetMonthlyBudget(selectedMonth.MonthNumber, selectedYear);
        }

        public async Task GetAllMonthlyBudgetsForUser() {

            allMonthlyBudgetsForUser = await monthlyBudgetService.GetAllMonthlyBudgetDatesForUser();
            
        }
        public void GetAvailableYearsForUser() {
            
            years = allMonthlyBudgetsForUser.OrderBy(x => x.Year)
            .Select(x => x.Year)
            .Distinct()
            .ToList();
        }

        public async Task GetSpecificMonthlyBudget() {

            appState.monthlyBudgetModel = await monthlyBudgetService.GetMonthlyBudget(selectedMonth.MonthNumber, selectedYear);
        }

        public void GetMonthsForSelectedYear() {

            var months = allMonthlyBudgetsForUser.Where(x => x.Year == selectedYear)
            .OrderBy(x => x.Month)
            .Select(x => x.Month)
            .Distinct()
            .ToList();

            monthsForYear.Clear();
            months.ForEach(delegate(int month) {
                monthsForYear.Add(
                    new MonthModel {
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                        MonthNumber = month
                    }
                );           
            });
        }
    }
}