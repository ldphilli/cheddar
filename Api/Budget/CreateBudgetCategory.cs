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
  public static class CreateBudgetCategory
  {

    private static jwtManagementToken manageToken = new jwtManagementToken();

    [FunctionName("CreateBudgetCategory")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetCategoriesContainerName,
                Connection = "CosmosDBConnection")]IAsyncCollector<BudgetCategoriesModel> documentsOut,
        ILogger log)
    {

      // Parse json back to budget line item model type
      var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      var item = JsonConvert.DeserializeObject<BudgetCategoriesModel>(requestBody);
      string userId = manageToken.GetUserIdFromToken("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL2NoZWRkYXJhcHAuYjJjbG9naW4uY29tLzlhMmQ5NWVmLTQwYjgtNDAwNC1iMWFhLWIyODYwZGM0NGIxYi92Mi4wLyIsImV4cCI6MTY1MzM4Mzk2NSwibmJmIjoxNjUzMzgwMzY1LCJhdWQiOiJlNzQwZmU5Zi1iMzE3LTRhYTUtODE5Ni05YmU1OGUwYjMwZTMiLCJzdWIiOiI1YWVjYjk0YS1hYzRjLTQ2ZWYtYTFlZC02N2E1YWQyMTBmNDciLCJnaXZlbl9uYW1lIjoiTHVrZSIsInRmcCI6IkIyQ18xX0NoZWRkYXJTaWduSW5TaWduVXAiLCJub25jZSI6IjYzNzQzYzZhLTYxNDAtNDY4MC1iZGEwLTNkZDI2ZWRhNTE1YiIsImF6cCI6ImU3NDBmZTlmLWIzMTctNGFhNS04MTk2LTliZTU4ZTBiMzBlMyIsInZlciI6IjEuMCIsImlhdCI6MTY1MzM4MDM2NX0.ZARNA6snDebC-MyiQpK5ZcbvK44u6HvlQLQAR0lWdEYD98k4Z_j_ewpSVZza0MM0WZDFPi8x4_53CZ5U7wKInqCBjj6LkYGSXCJW2zMxgY5sEP6A_3dJFgLY4Xk7rbfwewjJluWEhUEseuzY0Wh7TJfCDq1xrGRYZPp_IlYFJ_xjFbSmfWM7Mmrb3ZWDBl-NthR8aeFhoOkt9DG2qKeI72DlxNv1AzujGIbr9weeXmP_AiuLi9QcTSbERzwfHudmeQfZA2C_GvTd-Qs2c5LeI88ZFisSkXOVPd7UyoLq4ctTzQ2lOT32ZirE_pmN3-oSMoHe1PfJe7uU2BvGLlPAOg");
      if(userId != null || userId != string.Empty) {
        item.UserId = userId;
      }
      log.LogInformation("C# HTTP trigger function processed a request.");

      //Container container = cosmosClient.GetContainer(DatabaseId, ContainerId);
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