using Cheddar.Client.Models;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class SalaryUpdateVM {

        
        public async Task AddItemsToContainerAsync(ISalaryChanges salaryItem, NavigationManager nvm) {
            HttpClient client = new HttpClient();
            var url = "http://localhost:7071/api/CreateBudgetLineItem";
            await client.PostAsJsonAsync(url, salaryItem);
            nvm.NavigateTo("/budget");
        }
    }
}