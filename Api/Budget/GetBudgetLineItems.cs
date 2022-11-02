using Cheddar.Api.Shared;
using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cheddar.Api.Configuration;

namespace Cheddar.Function
{
    public static class GetBudgetLineItems
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private static jwtManagementToken manageToken = new jwtManagementToken();

        private static FeedIterator? streamResultSet;

        [FunctionName("GetBudgetLineItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetLineItemsContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {
            try {

                if (!req.Headers.TryGetValue("Authorization", out var token))
                {
                    return new BadRequestObjectResult("No token found");
                }

                Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.BudgetLineItemsContainerName);

                log.LogInformation("C# HTTP trigger function processed a request on GetBudgetLineItems.");

                string userId = manageToken.GetUserIdFromToken(token.ToString().Replace("Bearer ", ""));
                if(string.IsNullOrWhiteSpace(userId))
                {
                    throw new Exception("User Id is blank.");
                }

                //Setup query to database, get budget setting for current user
                log.LogInformation("Trying to find items");

                var budgetLineItems = await GetBudgetLineItemData(container, userId);

                if(budgetLineItems == null)
                {
                    return new BadRequestObjectResult("Failed to find records.");
                }

                return new OkObjectResult(budgetLineItems);
            }
            catch (CosmosException cosmosException)
            { //when (ex.Status == (int)HttpStatusCode.NotFound)
                return new BadRequestObjectResult($"Failed to read items. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }          
        }

        public static async Task<IEnumerable<BudgetLineItemModel>> GetBudgetLineItemData(Container container, string userId)
        {
            var budgetLineItems = container
                .GetItemLinqQueryable<BudgetLineItemModel>(true)
                .Where(x => x.UserId == userId)
                .AsEnumerable();

                if(budgetLineItems == null)
                {
                    return null;
                }

                return budgetLineItems;
        }
    }
}
