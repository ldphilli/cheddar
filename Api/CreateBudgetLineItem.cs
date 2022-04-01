using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Cheddar.Client.Models;

namespace Cheddar.Function
{
    public static class CreateBudgetLineItem
    {
        private const string EndpointUrl = "https://personal-finance-db.documents.azure.com:443/";
        private const string AuthorizationKey = "uKehVT4myAIG69BAYyLZOzHlxLh4Wx0JotaD0OQeg54lrcsWR8vQLpkAnfIKCv0j6Cd5hSCco26oyD9pQFbgwA==";
        [FunctionName("CreateBudgetLineItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var budgetLineItem = JsonConvert.DeserializeObject<BudgetLineItemModel>(requestBody);
            log.LogInformation("C# HTTP trigger function processed a request.");

            CosmosClient cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey);

            const string DatabaseId = "Cheddar";
            const string ContainerId = "BudgetLineItems";

            Container container = cosmosClient.GetContainer(DatabaseId, ContainerId);
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<BudgetLineItemModel> newBudgetLineItemResponse = await container.ReadItemAsync<BudgetLineItemModel>(budgetLineItem.Id, new PartitionKey(budgetLineItem.BLIid));
                Console.WriteLine("Item in database with id: {0} already exists\n", newBudgetLineItemResponse.Resource.Id);
            }
            catch(Exception ex) //when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<BudgetLineItemModel> newBudgetLineItemResponse = await container.CreateItemAsync<BudgetLineItemModel>(budgetLineItem, new PartitionKey(budgetLineItem.BLIid));
                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse.
                Console.WriteLine("Created item in database with id: {0}\n", newBudgetLineItemResponse.Resource.Id);
            }

            return new OkObjectResult("Success!");
        }
    }
}
