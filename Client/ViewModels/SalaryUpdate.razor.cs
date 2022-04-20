using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class SalaryUpdateViewModel {

        private readonly HttpClient ApiClient;

        public ISalaryUpdateModel salaryUpdateModel { get; set; }
        public List<ISalaryUpdateModel>? salaryUpdateItems = new List<ISalaryUpdateModel>();
        public Dictionary<string, double> allSalaryItems { get; set; }
        public SalaryUpdateViewModel(HttpClient apiClient)
        {
            salaryUpdateModel = new ISalaryUpdateModel();
            salaryUpdateModel.Id = Guid.NewGuid().ToString();
            salaryUpdateModel.UserId = 1;
            ApiClient = apiClient;
        }
        
        public async Task AddItemsToContainerAsync(ISalaryUpdateModel salaryItem, NavigationManager nvm) {

            await ApiClient.PostAsJsonAsync("api/CreateSalaryChangeItem?", salaryItem);
            nvm.NavigateTo("/budget");
        }

        public async Task GetSalaryUpdateItems() {

            salaryUpdateItems = await ApiClient.GetFromJsonAsync<List<ISalaryUpdateModel>>("api/GetSalaryUpdateItems?");
        }  
    }
}