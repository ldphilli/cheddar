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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cheddar.Api.Configuration;

namespace Cheddar.Function
{
  public static class UpdateMonthlyBudget
  {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        [FunctionName("UpdateMonthlyBudget")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetSettingsContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
        ILogger log) {

            try {

                Console.WriteLine("\n1.6 - Patching a item using its Id");

                Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.MonthlyBudgetContainerName);

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var item = JsonConvert.DeserializeObject<MonthlyBudgetModel>(requestBody);

                ItemResponse<MonthlyBudgetModel> response = await container.PatchItemAsync<MonthlyBudgetModel>(
                id: item.Id,
                partitionKey: new PartitionKey(item.UserId),
                patchOperations: new[] { PatchOperation.Replace("/Income", item.Income) });

                MonthlyBudgetModel updatedMonthlyBudget = response.Resource;
                log.LogInformation($"Income of updated item: {updatedMonthlyBudget.Income}");
                return new OkObjectResult(updatedMonthlyBudget);
            }
            catch(CosmosException cosmosException) {
                return new BadRequestObjectResult(cosmosException);
            }
        }
    }
}