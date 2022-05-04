using Cheddar.Shared.Models;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class BudgetLineItemVM {
        public BudgetLineItemModel bliModel { get; set; }
        public List<BudgetCategoriesModel> budgetCategories = new List<BudgetCategoriesModel>();
        
        public List<PaymentMethodsModel> paymentMethods = new List<PaymentMethodsModel>();
        private readonly HttpClient ApiClient;
        public BudgetLineItemVM(HttpClient apiClient)
        {
            ApiClient = apiClient;
            bliModel = new BudgetLineItemModel();
            bliModel.Id = Guid.NewGuid().ToString();
            bliModel.UserId = 2;
        }

        public async Task GetBudgetCatgories() {

            budgetCategories = await ApiClient.GetFromJsonAsync<List<BudgetCategoriesModel>>("api/GetBudgetCategoriesForUser?");
        }

        public async Task GetPaymentMethods() {

            paymentMethods = await ApiClient.GetFromJsonAsync<List<PaymentMethodsModel>>("api/GetPaymentMethodsForUser?");
        }

    }
}