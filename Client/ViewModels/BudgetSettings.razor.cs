using Cheddar.Shared.Models;
using Cheddar.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetSettingsViewModel {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;
        public BudgetCategoriesModel budgetCategoryModel { get; set; }
        public IPaymentMethodsModel paymentMethodModel { get; set; }
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
            budgetCategoryModel.UserId = 2;

            paymentMethodModel = new IPaymentMethodsModel();
            paymentMethodModel.UserId = 2;
            paymentMethodModel.Id = Guid.NewGuid().ToString();

            appState.budgetSettingsModel = new BudgetSettingsModel();
            budgetSettingsModel.userId = 2;
            budgetSettingsModel.Id = Guid.NewGuid().ToString();
        }

        public async Task AddBudgetCategoryToContainerAsync(BudgetCategoriesModel budgetCategory, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/CreateBudgetCategory", budgetCategory);
            nvm.NavigateTo("/budget");
        }

        public async Task AddPaymentMethodToContainerAsync(IPaymentMethodsModel paymentMethod, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/CreatePaymentMethod", paymentMethod);
            nvm.NavigateTo("/budget");
        }

        public async Task GetBudgetSettingDataForUser() {

            appState.budgetSettingsModel = await budgetSettingsService.GetMonthlyIncome();
        }

        public async Task CreateOrUpdateMonthlyIncome() {

            //await ApiClient.PostAsJsonAsync("api/CreateMonthlyBudgetSettings", budgetSettings);
            await budgetSettingsService.UpsertBudgetSettings(budgetSettingsModel);
            await GetBudgetSettingDataForUser();
            nvm.NavigateTo("/budget");
        }

        /*public async Task UpsertBudgetSettings(IBudgetSettingsModel budgetSettings, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/UpdateBudgetSettings", budgetSettings);
            nvm.NavigateTo("/budget");
        }*/
    }
}