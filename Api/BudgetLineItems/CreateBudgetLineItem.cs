using Cheddar.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cheddar.Function {
    public static class CreateBudgetLineItem {

        //private const string EndpointUrl = "https://personal-finance-db.documents.azure.com:443/";
        //private const string AuthorizationKey = "uKehVT4myAIG69BAYyLZOzHlxLh4Wx0JotaD0OQeg54lrcsWR8vQLpkAnfIKCv0j6Cd5hSCco26oyD9pQFbgwA==";
        [FunctionName("CreateBudgetLineItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: IDBOptionsModel.DBName,
                containerName: IDBOptionsModel.BudgetLineItemsContainerName,
                Connection = "CosmosDBConnection")]IAsyncCollector<BudgetLineItemModel> documentsOut,
            ILogger log) {

            // Parse json back to budget line item model type
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<BudgetLineItemModel>(requestBody);
            log.LogInformation("C# HTTP trigger function processed a request.");

            //Container container = cosmosClient.GetContainer(DatabaseId, ContainerId);
            try {

                await documentsOut.AddAsync(item);

            }
            catch(Exception ex) {//when (ex.Status == (int)HttpStatusCode.NotFound)
                
            }
            return new OkObjectResult("Success!");
        }
    }
}