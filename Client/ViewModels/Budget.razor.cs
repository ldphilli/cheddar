using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Azure.Cosmos;
//using Newtonsoft.Json;
using Cheddar.Client.Models;

namespace Cheddar.Client.ViewModels {
    public class BudgetVM {

        /// <summary>
        /// Add BudgetLineItem items to the container
        /// </summary>
        public async Task AddItemsToContainerAsync(BudgetLineItemModel budgetLineItem) {
            HttpyClient client = new HttpClient();
            var url = "http://localhost:7071/api/CreateBudgetLineItem";
            await client.PostAsJsonAsync(url, budgetLineItem);
        }
    }
}

