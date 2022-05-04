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
using System.Threading.Tasks;
using Cheddar.Api.Configuration;

namespace Cheddar.Function {
    public static class GetRemainingExpenditureCategories {
        private static readonly JsonSerializer Serializer = new JsonSerializer();
        
        [FunctionName("GetRemainingExpenditureCategoriesForUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.RemainingExpenditureCategoriesContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log) {
            
            Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.RemainingExpenditureCategoriesContainerName);

            try {
                List<RemainingExpenditureCategoriesModel> allRemainingExpenditureCategoriesForUser = await GetRemainingExpenditureCategoryData(container);
                return new OkObjectResult(allRemainingExpenditureCategoriesForUser);
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

        public static async Task<List<RemainingExpenditureCategoriesModel>> GetRemainingExpenditureCategoryData(Container container) {
            
            List<RemainingExpenditureCategoriesModel> allRemainingExpenditureCategoriesForUser = new List<RemainingExpenditureCategoriesModel>();

                //Setup query to database, get all budget line items for current user
                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c where c.UserId = @userId")
                .WithParameter("@userId", 2);
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
                            List<RemainingExpenditureCategoriesModel> remainingExpenditureCategories = streamResponse.Documents.ToObject<List<RemainingExpenditureCategoriesModel>>();
                            allRemainingExpenditureCategoriesForUser.AddRange(remainingExpenditureCategories);
                        }
                        //If no results are returned
                        else {
                            Console.WriteLine($"Read all items from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                        }
                    }
                }
            return allRemainingExpenditureCategoriesForUser;
        }
    }
}