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
    public static class GetBudgetCategories
    {

        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private static jwtManagementToken manageToken = new jwtManagementToken();

        [FunctionName("GetBudgetCategoriesForUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetCategoriesContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {
            try {

                if (!req.Headers.TryGetValue("Authorization", out var token))
                {
                    return new BadRequestObjectResult("No token found");
                }

                Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.BudgetCategoriesContainerName);

                log.LogInformation("C# HTTP trigger function processed a request on GetBudgetCategories.");

                string userId = manageToken.GetUserIdFromToken(token.ToString().Replace("Bearer ", ""));
                if(string.IsNullOrWhiteSpace(userId))
                {
                    throw new Exception("User Id is blank.");
                }

                //Setup query to database, get budget categories for current user
                log.LogInformation("Trying to find items");

                var budgetCategories = container
                .GetItemLinqQueryable<BudgetCategoriesModel>(true)
                .Where(x => x.UserId == userId)
                .AsEnumerable();

                if(budgetCategories == null)
                {
                    return new BadRequestObjectResult("Failed to find records.");
                }

                return new OkObjectResult(budgetCategories);
            }
            catch (CosmosException cosmosException)
            { //when (ex.Status == (int)HttpStatusCode.NotFound)
                return new BadRequestObjectResult($"Failed to read items. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }
        }
    }
}
