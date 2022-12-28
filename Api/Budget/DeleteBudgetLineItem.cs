using Cheddar.Api.Shared;
using Cheddar.Shared.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Cheddar.Api.Configuration;

namespace Cheddar.Function
{
    public static class DeleteBudgetLineItem
    {

        private static jwtManagementToken manageToken = new jwtManagementToken();

        [FunctionName("DeleteBudgetLineItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetLineItemsContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {

            if (!req.Headers.TryGetValue("Authorization", out var token))
            {
                return new BadRequestObjectResult("No token found");
            }

            // Parse json back to budget line item model type
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<BudgetLineItemModel>(requestBody);
            
            Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.MonthlyBudgetContainerName);

            string userId = manageToken.GetUserIdFromToken(token.ToString().Replace("Bearer ", ""));
            if(string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("User Id is blank.");
            }
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {

                ItemResponse<BudgetLineItemModel> response = await container.DeleteItemAsync<BudgetLineItemModel>(
                partitionKey: new PartitionKey("/UserId"),
                id: item.Id);

            }
            catch (Exception ex)
            {//when (ex.Status == (int)HttpStatusCode.NotFound)

            }
            return new OkObjectResult("Success!");
        }
    }
}
