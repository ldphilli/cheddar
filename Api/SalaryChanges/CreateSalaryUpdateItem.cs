using Cheddar.Shared.Models;
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
using Cheddar.Api.Configuration;

namespace Cheddar.Function {
    public static class CreateSalaryChangeItem {

        [FunctionName("CreateSalaryChangeItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.SalaryUpdateItemsContainerName,
                Connection = "CosmosDBConnection")]IAsyncCollector<ISalaryUpdateModel> documentsOut,
            ILogger log) {

            // Parse json back to budget line item model type
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<ISalaryUpdateModel>(requestBody);
            log.LogInformation("C# HTTP trigger function processed a request.");

            await documentsOut.AddAsync(item);

            return new OkObjectResult("Success!");
        }
    }
}
