using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Azure.Cosmos;

namespace Cheddar.Client.Services {
    public partial class Budget {
        private const string EndpointUrl = "https://personal-finance-db.documents.azure.com:443/";
        private const string AuthorizationKey = "uKehVT4myAIG69BAYyLZOzHlxLh4Wx0JotaD0OQeg54lrcsWR8vQLpkAnfIKCv0j6Cd5hSCco26oyD9pQFbgwA==";
        private const string DatabaseId = "Cheddar";
        private const string ContainerId = "BudgetLineItems";

        public class BudgetLineItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            public string BudgetLineName { get; set; }
            public double Cost { get; set; }
            public string Category { get; set; }
            public string PaymentMethod { get; set; }
            public string ContractEndDate { get; set; }
            public override string ToString()
            {
                return JsonSerializer.Serialize(this);
            }
        }

        static async Task Main(string[] args)
        {
            CosmosClient cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey);
            await Budget.AddItemsToContainerAsync(cosmosClient);
        }
        /// <summary>
        /// Add BudgetLineItem items to the container
        /// </summary>
        public static async Task AddItemsToContainerAsync(CosmosClient cosmosClient)
        {

            const string DatabaseId = "Cheddar";
            const string ContainerId = "BudgetLineItems";
            // Create a BudgetLineItem object for the Andersen BudgetLineItem
            BudgetLineItem newBudgetLineItem = new BudgetLineItem
            {
                Id = "2",
                BudgetLineName = "Adobe Creative Cloud",
                Cost = 16.24,
                Category = "Enteratinment",
                PaymentMethod = "Paypal",
                ContractEndDate = "Rolling month"
            };

            CosmosContainer container = cosmosClient.GetContainer(DatabaseId, ContainerId);
            try
            {

                // Read the item to see if it exists.  
                ItemResponse<BudgetLineItem> newBudgetLineItemResponse = await container.ReadItemAsync<BudgetLineItem>(newBudgetLineItem.Id, new Azure.Cosmos.PartitionKey(newBudgetLineItem.Id));
                Console.WriteLine("Item in database with id: {0} already exists\n", newBudgetLineItemResponse.Value.Id);
            }
            catch(CosmosException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                //ItemResponse<BudgetLineItem> newBudgetLineItemResponse = await container.CreateItemAsync<BudgetLineItem>(newBudgetLineItem, new Azure.Cosmos.PartitionKey(newBudgetLineItem.Id));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse.
                //Console.WriteLine("Created item in database with id: {0}\n", newBudgetLineItemResponse.Value.Id);
            }
        }
    }
}

