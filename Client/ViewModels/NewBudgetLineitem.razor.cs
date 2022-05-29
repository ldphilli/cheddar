using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetLineItemVM {
        public BudgetLineItemModel bliModel { get; set; }
        public List<BudgetCategoriesModel> budgetCategories = new List<BudgetCategoriesModel>();
        
        public List<PaymentMethodsModel> paymentMethods = new List<PaymentMethodsModel>();
        private readonly HttpClient ApiClient;

        private readonly ApplicationState appState;
        public BudgetLineItemVM(HttpClient apiClient, ApplicationState applicationState)
        {
            ApiClient = apiClient;
            bliModel = new BudgetLineItemModel();
            bliModel.Id = Guid.NewGuid().ToString();
            appState = applicationState;
        }

        public async Task GetBudgetCatgories() {

            string request = String.Concat("api/GetBudgetCategoriesForUser?claim=", appState.Token);
            budgetCategories = await ApiClient.GetFromJsonAsync<List<BudgetCategoriesModel>>(request);
        }

        public async Task GetPaymentMethods() {

            string request = String.Concat("api/GetPaymentMethodsForUser?claim=", appState.Token);
            paymentMethods = await ApiClient.GetFromJsonAsync<List<PaymentMethodsModel>>(request);
        }

    }
}