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

        public MonthlyBudgetViewModel(HttpClient apiClient, NavigationManager navManager, ApplicationState applicationState, MonthlyBudgetService mbService)
        {
            ApiClient = apiClient;
            nvm = navManager;
            appState = applicationState;
            monthlyBudgetService = mbService;
        }

        public async Task GetLatestMonthlyBudgetForUser() {
            
            appState.monthlyBudgetModel = await monthlyBudgetService.GetLatestMonthlyBudget();
            /*DateTime today = DateTime.Now;
            int month = today.Month;
            int year = today.Year;
            appState.monthlyBudgetModel = await monthlyBudgetService.GetMonthlyBudget(month, year);
            if(appState.monthlyBudgetModel == null)
            {
                appState.monthlyBudgetModel.Income = 0;
                appState.monthlyBudgetModel.Outgoing = 0;
                appState.monthlyBudgetModel.Remaining = 0;
            }*/
        }

        public async Task GetAvailableYearsForUser() {

            List<MonthlyBudgetModel> allMonthlyBudgetsForUser = await monthlyBudgetService.GetAllMonthlyBudgetDatesForUser();
            years = allMonthlyBudgetsForUser.OrderBy(x => x.Year)
            .Select(x => x.Year)
            .Distinct()
            .ToList();
            
            selectedYear = years.LastOrDefault();

            var months = allMonthlyBudgetsForUser.Where(x => x.Year == years.FirstOrDefault())
            .OrderBy(x => x.Month)
            .Select(x => x.Month)
            .Distinct()
            .ToList();

            months.ForEach(delegate(int month) {
                monthsForYear.Add(
                    new MonthModel {
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                        MonthNumber = month
                    }
                    //monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)
                );           
            });
            
            selectedMonth = monthsForYear.LastOrDefault();

        }

        public async Task GetSpecificMonthlyBudget() {

            appState.monthlyBudgetModel = await monthlyBudgetService.GetMonthlyBudget(selectedMonth.MonthNumber, selectedYear);
        }
    }
}