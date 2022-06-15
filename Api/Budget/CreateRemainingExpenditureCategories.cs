using Cheddar.Api.Shared;
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
    public static class CreateRemainingExpenditureCategories
    {

        private static jwtManagementToken manageToken = new jwtManagementToken();

        [FunctionName("CreateRemainingExpenditureCategories")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.RemainingExpenditureCategoriesContainerName,
                Connection = "CosmosDBConnection")]IAsyncCollector<RemainingExpenditureCategoriesModel> documentsOut,
            ILogger log)
        {

            if (!req.Headers.TryGetValue("Authorization", out var token))
            {
                return new BadRequestObjectResult("No token found");
            }

            // Parse json back to budget line item model type
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<RemainingExpenditureCategoriesModel>(requestBody);
            string userId = manageToken.GetUserIdFromToken(token.ToString().Replace("Bearer ", ""));
            if (userId != null || userId != string.Empty)
            {
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