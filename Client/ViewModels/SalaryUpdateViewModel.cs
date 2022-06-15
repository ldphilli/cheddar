using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels
{
    public class SalaryUpdateViewModel
    {

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

        public async Task AddItemsToContainerAsync(SalaryUpdateModel salaryItem, NavigationManager nvm)
        {
            await ApiClient.PostAsJsonAsync("api/CreateSalaryChangeItem", salaryItem);
            nvm.NavigateTo("/budget");
        }

        public async Task GetSalaryUpdateItems()
        {
            salaryUpdateItems = await ApiClient.GetFromJsonAsync<List<SalaryUpdateModel>>("api/GetSalaryUpdateItems");
        }
    }
}