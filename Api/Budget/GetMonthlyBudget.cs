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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cheddar.Api.Configuration;

namespace Cheddar.Function {
    public static class GetMonthlyBudget {

        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private static jwtManagementToken manageToken = new jwtManagementToken();
        
        [FunctionName("GetMonthlyBudget")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetSettingsContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log) {

            string token = req.Query["claim"];
            if(token != null) {
                log.LogInformation(token);
            } else {
                return new BadRequestObjectResult("No token found");
            }

            string month = req.Query["month"];
            string year = req.Query["year"];
            
            Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.MonthlyBudgetContainerNamer);

            log.LogInformation("C# HTTP trigger function processed a request on GetMonthlyIncome.");
            try {
                List<MonthlyBudgetModel> allMonthlyBudgetItemsForUser = new List<MonthlyBudgetModel>();
                string userId = manageToken.GetUserIdFromToken(token);
                if(userId != null || userId != string.Empty) {
                //Setup query to database, get all budget line items for current user
                    QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c where c.userId = @userId and c.Month = @month and c.Year = @year")
                    .WithParameter("@userId", userId)
                    .WithParameter("@month", month)
                    .WithParameter("@year", year);
                    using (FeedIterator streamResultSet = container.GetItemQueryStreamIterator(
                        queryDefinition,
                        null,
                        new QueryRequestOptions()
                    ))

                    //While the stream has more results (0 or more)
                    while (streamResultSet.HasMoreResults) {
                        using (ResponseMessage responseMessage = await streamResultSet.ReadNextAsync()) {
                            // Item stream operations do not throw exceptions for better performance
                            if (responseMessage.IsSuccessStatusCode) {
                                //Parse return to list of Budget Line Item Model
                                dynamic streamResponse = FromStream<dynamic>(responseMessage.Content);
                                List<MonthlyBudgetModel> monthlyBudgetItems = streamResponse.Documents.ToObject<List<MonthlyBudgetModel>>();
                                if(monthlyBudgetItems != null) {
                                    allMonthlyBudgetItemsForUser.AddRange(monthlyBudgetItems);
                                }
                            }
                            //If no results are returned
                            else {
                                Console.WriteLine($"Read all items from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                            }
                        }
                    }
                }
                
                if(allMonthlyBudgetItemsForUser != null)
                {
                    return new OkObjectResult(allMonthlyBudgetItemsForUser.First());
                }
                else{
                    return new BadRequestObjectResult("Failed to find records.");
                }
                
            }
            catch(CosmosException cosmosException) { //when (ex.Status == (int)HttpStatusCode.NotFound)
                return new BadRequestObjectResult($"Failed to read items. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        private static T FromStream<T>(Stream stream) {
            using (stream) {
                if (typeof(Stream).IsAssignableFrom(typeof(T))) {
                    return (T)(object)stream;
                }

                using (StreamReader sr = new StreamReader(stream)) {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(sr)) {
                        return Serializer.Deserialize<T>(jsonTextReader);
                    }
                }
            }
        }
    }
}