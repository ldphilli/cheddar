using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetSettingsViewModel {

        private readonly HttpClient ApiClient;
        public IBudgetCategoriesModel budgetCategoryModel { get; set; }
        public IPaymentMethodsModel paymentMethodModel { get; set; }

        public BudgetSettingsViewModel(HttpClient apiClient)
        {
            ApiClient = apiClient;

            budgetCategoryModel = new IBudgetCategoriesModel();
            budgetCategoryModel.Id = Guid.NewGuid().ToString();
            budgetCategoryModel.UserId = 2;

            paymentMethodModel = new IPaymentMethodsModel();
            paymentMethodModel.UserId = 2;
            paymentMethodModel.Id = Guid.NewGuid().ToString();
        }

        public async Task AddBudgetCategoryToContainerAsync(IBudgetCategoriesModel budgetCategory, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/CreateBudgetCategory", budgetCategory);
            nvm.NavigateTo("/budget");
        }

        public async Task AddPaymentMethodToContainerAsync(IPaymentMethodsModel paymentMethod, NavigationManager nvm) {
            Console.WriteLine(paymentMethod.UserId);
            await ApiClient.PostAsJsonAsync("api/CreatePaymentMethod", paymentMethod);
            nvm.NavigateTo("/budget");
        }
    }
}