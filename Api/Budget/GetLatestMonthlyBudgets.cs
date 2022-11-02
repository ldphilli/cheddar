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
using System.Linq;
using Cheddar.Api.Configuration;

namespace Cheddar.Function {
    public static class GetLatestMonthlyBudget {

        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private static jwtManagementToken manageToken = new jwtManagementToken();
        
        [FunctionName("GetLatestMonthlyBudget")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetSettingsContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log) {
            try {

                if (!req.Headers.TryGetValue("Authorization", out var token))
                {
                    return new BadRequestObjectResult("No token found");
                }
                
                Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.MonthlyBudgetContainerName);

                log.LogInformation("C# HTTP trigger function processed a request on GetMonthlyBudget.");

                string userId = manageToken.GetUserIdFromToken(token.ToString().Replace("Bearer ", ""));
                if(string.IsNullOrWhiteSpace(userId))
                {
                    throw new Exception("User Id is blank.");
                }

                //Setup query to database, get all budget line items for current user
                log.LogInformation("Trying to find items");

                var monthBudget = container
                    .GetItemLinqQueryable<MonthlyBudgetModel>(true)
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .AsEnumerable()
                    .FirstOrDefault();

                if(monthBudget == null)
                {
                    return new BadRequestObjectResult("Failed to find records.");
                }

                return new OkObjectResult(monthBudget);
            }
            catch(CosmosException cosmosException) { //when (ex.Status == (int)HttpStatusCode.NotFound)
                return new BadRequestObjectResult($"Failed to read items. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }
        }
    }
}
