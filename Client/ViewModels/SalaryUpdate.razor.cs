using Cheddar.Client.Models;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class SalaryUpdateViewModel {

        public ISalaryUpdateModel salaryUpdateModel { get; set; }
        public List<ISalaryUpdateModel>? salaryUpdateItems = new List<ISalaryUpdateModel>();
        public Dictionary<string, double> allSalaryItems { get; set; }
        public SalaryUpdateViewModel()
        {
            salaryUpdateModel = new ISalaryUpdateModel();
            salaryUpdateModel.Id = Guid.NewGuid().ToString();
            salaryUpdateModel.UserId = 1;
        }
        
        public async Task AddItemsToContainerAsync(ISalaryUpdateModel salaryItem, NavigationManager nvm) {
            HttpClient client = new HttpClient();
            var url = "https://cheddarapi.azurewebsites.net/api/CreateSalaryChangeItem?";
            await client.PostAsJsonAsync(url, salaryItem);
            nvm.NavigateTo("/budget");
        }

        public async Task GetSalaryUpdateItems() {

            HttpClient client = new HttpClient();
            var url = "https://cheddarapi.azurewebsites.net/api/GetSalaryUpdateItems?";
            salaryUpdateItems = await client.GetFromJsonAsync<List<ISalaryUpdateModel>>(url);
        }  
    }
}