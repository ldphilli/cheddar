using Cheddar.Client.Models;
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

namespace Cheddar.Function {
    public static class GetBudgetLineItems {
        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private const string EndpointUrl = "https://personal-finance-db.documents.azure.com:443/";
        private const string AuthorizationKey = "uKehVT4myAIG69BAYyLZOzHlxLh4Wx0JotaD0OQeg54lrcsWR8vQLpkAnfIKCv0j6Cd5hSCco26oyD9pQFbgwA==";
        [FunctionName("GetBudgetLineItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log) {
            //Database setup
            CosmosClient cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey);
            const string DatabaseId = "Cheddar";
            const string ContainerId = "BudgetLineItems";
            Container container = cosmosClient.GetContainer(DatabaseId, ContainerId);

            try {
                List<BudgetLineItemModel> allBudgetLineItemsForUser = new List<BudgetLineItemModel>();

                //Setup query to database, get all budget line items for current user
                var queryDefinition = new QueryDefinition("SELECT * FROM c where c.UserId = @userId")
                .WithParameter("@userId", 1);
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
                            List<BudgetLineItemModel> budgetLineItems = streamResponse.Documents.ToObject<List<BudgetLineItemModel>>();
                            allBudgetLineItemsForUser.AddRange(budgetLineItems);
                        }
                        //If no results are returned
                        else {
                            Console.WriteLine($"Read all items from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                        }
                    }
                }
                return new OkObjectResult(allBudgetLineItemsForUser);
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
