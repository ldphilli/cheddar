using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class SalaryUpdateViewModel {

        private readonly HttpClient ApiClient;

        public SalaryUpdateModel salaryUpdateModel { get; set; }
        public List<SalaryUpdateModel>? salaryUpdateItems = new List<SalaryUpdateModel>();
        public Dictionary<string, double> allSalaryItems { get; set; }
        public SalaryUpdateViewModel(HttpClient apiClient)
        {
            salaryUpdateModel = new SalaryUpdateModel();
            salaryUpdateModel.Id = Guid.NewGuid().ToString();
            salaryUpdateModel.UserId = 1;
            ApiClient = apiClient;
        }
        
        public async Task AddItemsToContainerAsync(SalaryUpdateModel salaryItem, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/CreateSalaryChangeItem?", salaryItem);
            nvm.NavigateTo("/budget");
        }

        public async Task GetSalaryUpdateItems() {

            salaryUpdateItems = await ApiClient.GetFromJsonAsync<List<SalaryUpdateModel>>("api/GetSalaryUpdateItems?");
        }  
    }
}