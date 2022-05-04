using Cheddar.Shared.Models;
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
  public static class CreateOrUpdateBudgetSettings
  {

    [FunctionName("CreateOrUpdateBudgetSettingsDoc")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetSettingsContainerName,
                Connection = "CosmosDBConnection")]IAsyncCollector<BudgetSettingsModel> documentsOut,
        ILogger log)
    {

      // Parse json back to budget line item model type
      var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      var item = JsonConvert.DeserializeObject<BudgetSettingsModel>(requestBody);
      log.LogInformation("C# HTTP trigger function processed a request.");

      try
      {
        await documentsOut.AddAsync(item);
      }
      catch (Exception ex)
      {//when (ex.Status == (int)HttpStatusCode.NotFound)

      }
      return new OkObjectResult("Success!");
    }
  }
}