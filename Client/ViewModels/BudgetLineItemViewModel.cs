using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels
{
    public class BudgetLineItemViewModel
    {
        public BudgetLineItemModel bliModel { get; set; }
        public List<BudgetCategoriesModel> budgetCategories = new List<BudgetCategoriesModel>();

        public List<PaymentMethodsModel> paymentMethods = new List<PaymentMethodsModel>();
        private readonly HttpClient ApiClient;

        private readonly ApplicationState appState;
        public BudgetLineItemViewModel(HttpClient apiClient, ApplicationState applicationState)
        {
            ApiClient = apiClient;
            bliModel = new BudgetLineItemModel();
            bliModel.Id = Guid.NewGuid().ToString();
            appState = applicationState;
        }

        public async Task GetBudgetCatgories()
        {
            budgetCategories = await ApiClient.GetFromJsonAsync<List<BudgetCategoriesModel>>("api/GetBudgetCategoriesForUser");
        }

        public async Task GetPaymentMethods()
        {
            paymentMethods = await ApiClient.GetFromJsonAsync<List<PaymentMethodsModel>>("api/GetPaymentMethodsForUser");
        }

    }
}