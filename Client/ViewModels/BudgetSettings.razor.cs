using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetSettingsViewModel {

        private readonly HttpClient ApiClient;
        public BudgetCategoriesModel budgetCategoryModel { get; set; }
        public IPaymentMethodsModel paymentMethodModel { get; set; }
        public IBudgetSettingsModel budgetSettingsModel { get; set; }

        public BudgetSettingsViewModel(HttpClient apiClient)
        {
            ApiClient = apiClient;

            budgetCategoryModel = new BudgetCategoriesModel();
            budgetCategoryModel.Id = Guid.NewGuid().ToString();
            budgetCategoryModel.UserId = 2;

            paymentMethodModel = new IPaymentMethodsModel();
            paymentMethodModel.UserId = 2;
            paymentMethodModel.Id = Guid.NewGuid().ToString();

            budgetSettingsModel = new IBudgetSettingsModel();
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

        /*public async Task GetMonthlyIncomeForUser() {

            budgetSettingsModel = await ApiClient.GetFromJsonAsync<IBudgetSettingsModel>("api/GetMonthlyIncome?");
        }*/

        public async Task SetMonthlyBudgetSettings(IBudgetSettingsModel budgetSettings, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/CreateMonthlyBudgetSettings", budgetSettings);
            nvm.NavigateTo("/budget");
        }

        /*public async Task UpsertBudgetSettings(IBudgetSettingsModel budgetSettings, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/UpdateBudgetSettings", budgetSettings);
            nvm.NavigateTo("/budget");
        }*/
    }
}