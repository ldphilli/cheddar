using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Cheddar.Client.Models;

namespace Cheddar.Client.ViewModels {
    public class BudgetVM {

        /// <summary>
        /// Add BudgetLineItem items to the container
        /// </summary>
        public async Task AddItemsToContainerAsync(BudgetLineItemModel budgetLineItem, NavigationManager nvm) {
            HttpClient client = new HttpClient();
            var url = "http://localhost:7071/api/CreateBudgetLineItem";
            await client.PostAsJsonAsync(url, budgetLineItem);
            nvm.NavigateTo("/budget");
        }
    }
}

