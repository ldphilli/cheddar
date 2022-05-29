using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class SalaryUpdateViewModel {

        private readonly HttpClient ApiClient;
        private ApplicationState appState;

        public SalaryUpdateModel salaryUpdateModel { get; set; }
        public List<SalaryUpdateModel>? salaryUpdateItems = new List<SalaryUpdateModel>();
        public Dictionary<string, double> allSalaryItems { get; set; }
        public SalaryUpdateViewModel(HttpClient apiClient, ApplicationState applicationState)
        {
            salaryUpdateModel = new SalaryUpdateModel();
            salaryUpdateModel.Id = Guid.NewGuid().ToString();
            ApiClient = apiClient;
            appState = applicationState;
        }
        
        public async Task AddItemsToContainerAsync(SalaryUpdateModel salaryItem, NavigationManager nvm) {

            string request = String.Concat("api/CreateSalaryChangeItem?claim=", appState.Token);
            await ApiClient.PostAsJsonAsync(request, salaryItem);
            nvm.NavigateTo("/budget");
        }

        public async Task GetSalaryUpdateItems() {

            string request = String.Concat("api/GetSalaryUpdateItems?claim=", appState.Token);
            salaryUpdateItems = await ApiClient.GetFromJsonAsync<List<SalaryUpdateModel>>(request);
        }  
    }
}