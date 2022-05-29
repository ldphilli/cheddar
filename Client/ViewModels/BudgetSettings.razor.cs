using Cheddar.Shared.Models;
using Cheddar.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetSettingsViewModel {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;
        public BudgetCategoriesModel budgetCategoryModel { get; set; }
        public PaymentMethodsModel paymentMethodModel { get; set; }
        public RemainingExpenditureCategoriesModel remainingExpenditureCategoriesModel { get; set; }
        public BudgetSettingsModel budgetSettingsModel => appState.budgetSettingsModel;
        public BudgetSettingsService budgetSettingsService;

        private readonly NavigationManager nvm;

        public BudgetSettingsViewModel(HttpClient apiClient, NavigationManager navManager, ApplicationState applicationState, BudgetSettingsService bsService)
        {
            ApiClient = apiClient;
            nvm = navManager;
            appState = applicationState;
            budgetSettingsService = bsService;

            budgetCategoryModel = new BudgetCategoriesModel();
            budgetCategoryModel.Id = Guid.NewGuid().ToString();

            paymentMethodModel = new PaymentMethodsModel();
            paymentMethodModel.Id = Guid.NewGuid().ToString();

            appState.budgetSettingsModel = new BudgetSettingsModel();
            budgetSettingsModel.Id = Guid.NewGuid().ToString();

            remainingExpenditureCategoriesModel = new RemainingExpenditureCategoriesModel();
            remainingExpenditureCategoriesModel.Id = Guid.NewGuid().ToString();
            //remainingExpenditureCategoriesModel.UserId = 2;
        }

        public async Task AddBudgetCategoryToContainerAsync(BudgetCategoriesModel budgetCategory, NavigationManager nvm) {

            string request = String.Concat("api/CreateBudgetCategory?claim=", appState.Token);
            await ApiClient.PostAsJsonAsync("api/CreateBudgetCategory", budgetCategory);
            nvm.NavigateTo("/budget");
        }

        public async Task AddPaymentMethodToContainerAsync(PaymentMethodsModel paymentMethod, NavigationManager nvm) {
            
            string request = String.Concat("api/CreatePaymentMethod?claim=", appState.Token);
            await ApiClient.PostAsJsonAsync(request, paymentMethod);
            nvm.NavigateTo("/budget");
        }

        public async Task GetBudgetSettingDataForUser() {

            appState.budgetSettingsModel = await budgetSettingsService.GetMonthlyIncome();
        }

        public async Task CreateOrUpdateMonthlyIncome() {

            await budgetSettingsService.CreateOrUpdateBudgetSettingsDoc(budgetSettingsModel);
            await GetBudgetSettingDataForUser();
            nvm.NavigateTo("/budget");
        }

        public async Task CreateRemainingExpenditureCategory() {

            await budgetSettingsService.CreateRemainingExpenditureCategoriesDoc(remainingExpenditureCategoriesModel);
            nvm.NavigateTo("/budget");
        }
    }
}