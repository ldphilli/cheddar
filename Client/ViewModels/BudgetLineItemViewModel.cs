using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels
{
    public class BudgetLineItemViewModel
    {
        public BudgetLineItemModel bliModel { get; set; }
        public List<BudgetCategoriesModel> budgetCategories = new List<BudgetCategoriesModel>();

        public List<PaymentMethodsModel> paymentMethods = new List<PaymentMethodsModel>();
        private readonly HttpClient ApiClient;
        private readonly NavigationManager nvm;
        private readonly ApplicationState appState;
        public BudgetLineItemViewModel(HttpClient apiClient, ApplicationState applicationState, NavigationManager navManager)
        {
            ApiClient = apiClient;
            appState = applicationState;
            nvm = navManager;
            /*if(bliModel == null){
                bliModel = new BudgetLineItemModel();
                bliModel.Id = Guid.NewGuid().ToString();
                Console.WriteLine(bliModel.Id);
            }*/
        }

        public async Task CreateBudgetLineItem()
        {

            bliModel = new BudgetLineItemModel();
            bliModel.Id = Guid.NewGuid().ToString();
            await ApiClient.PostAsJsonAsync("api/CreateBudgetLineItem", bliModel);
            nvm.NavigateTo("/budget");
        }

        public async Task GetBudgetCatgories()
        {
            budgetCategories = await ApiClient.GetFromJsonAsync<List<BudgetCategoriesModel>>("api/GetBudgetCategoriesForUser");
        }

        public async Task GetPaymentMethods()
        {
            paymentMethods = await ApiClient.GetFromJsonAsync<List<PaymentMethodsModel>>("api/GetPaymentMethodsForUser");
        }

        public async Task DeleteBudgetLineItem(BudgetLineItemModel budgetLineItemToDelete)
        {
            Console.WriteLine(budgetLineItemToDelete.BudgetLineName);
            await ApiClient.PostAsJsonAsync<BudgetLineItemModel>("api/DeleteBudgetLineItem", budgetLineItemToDelete);
        }
    }
}