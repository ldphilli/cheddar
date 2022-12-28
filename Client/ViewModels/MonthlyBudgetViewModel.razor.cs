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
        public List<MonthModel> months = new List<MonthModel>();
        public int selectedYear;
        public MonthModel selectedMonth;
        public List<MonthlyBudgetModel> allMonthlyBudgetsForUser;
        public List<RemainingExpenditureCategoriesWithAmountModel> expenditureCategories;

        public MonthlyBudgetViewModel(HttpClient apiClient, NavigationManager navManager, ApplicationState applicationState, MonthlyBudgetService mbService)
        {
            ApiClient = apiClient;
            nvm = navManager;
            appState = applicationState;
            monthlyBudgetService = mbService;
        }

        public async Task SetupLatestMonthlyBudgetForUser() {
            
           await GetAllMonthlyBudgetsForUser();

           years = GetAvailableYearsForUser();
           selectedYear = years.LastOrDefault();

           months = GetMonthsForSelectedYear();
           selectedMonth = months.LastOrDefault();

           appState.monthlyBudgetModel = allMonthlyBudgetsForUser.OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .FirstOrDefault();

            expenditureCategories = appState.monthlyBudgetModel.expenditureCategories;
        }

        public void ReloadBudgetForSelectedYear() {

            months = GetMonthsForSelectedYear();
            selectedMonth = months.FirstOrDefault();
            appState.monthlyBudgetModel = GetSpecificMonthlyBudget();

        }

        public void ReloadBudgetForSelectedMonth() {

            appState.monthlyBudgetModel = GetSpecificMonthlyBudget();
        }

        public async Task GetAllMonthlyBudgetsForUser() {

            allMonthlyBudgetsForUser = await monthlyBudgetService.GetAllMonthlyBudgetDatesForUser();
            
        }
        public List<int> GetAvailableYearsForUser() {
            
            List<int> yearsWithAMonthlyBudget = allMonthlyBudgetsForUser.OrderBy(x => x.Year)
            .Select(x => x.Year)
            .Distinct()
            .ToList();

            return yearsWithAMonthlyBudget;
        }

        public MonthlyBudgetModel GetSpecificMonthlyBudget() {

            MonthlyBudgetModel monthlyBudgetRecord = allMonthlyBudgetsForUser.Where(x => x.Year == selectedYear && x.Month == selectedMonth.MonthNumber)
            .FirstOrDefault();

            return monthlyBudgetRecord;
        }

        public List<MonthModel> GetMonthsForSelectedYear() {

            List<MonthModel> monthsForYear = allMonthlyBudgetsForUser.Where(x => x.Year == selectedYear)
            .OrderBy(x => x.Month)
            .Select(x => 
                new MonthModel {
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                    MonthNumber = x.Month
                })
            .ToList();

            return monthsForYear;
        }

        public async Task UpdateMonthlyBudget() {

            appState.monthlyBudgetModel.Remaining = Math.Round(appState.monthlyBudgetModel.Income - appState.monthlyBudgetModel.Outgoing, 2);
            await monthlyBudgetService.UpdateMonthlyBudgetForUser(monthlyBudgetModel);
        }
    }
}