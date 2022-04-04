using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Cheddar.Client.Models;
using System.Collections;

namespace Cheddar.Function
{
    public static class GetBudgetLineItems
    {
        private const string EndpointUrl = "https://personal-finance-db.documents.azure.com:443/";
        private const string AuthorizationKey = "uKehVT4myAIG69BAYyLZOzHlxLh4Wx0JotaD0OQeg54lrcsWR8vQLpkAnfIKCv0j6Cd5hSCco26oyD9pQFbgwA==";
        [FunctionName("GetBudgetLineItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            //var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var budgetLineItem = JsonConvert.DeserializeObject<BudgetLineItemModel>(requestBody);
            //log.LogInformation("C# HTTP trigger function processed a request.");

            CosmosClient cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey);

            const string DatabaseId = "Cheddar";
            const string ContainerId = "BudgetLineItems";

            Container container = cosmosClient.GetContainer(DatabaseId, ContainerId);
            try
            {
                IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
                var queryDefinition = new QueryDefinition("SELECT * FROM c where c.UserId = @userId")
                .WithParameter("@userId", 1);
                FeedIterator iterator = container.GetItemQueryStreamIterator(
                    feedRanges[0],
                    queryDefinition,
                    null,
                    new QueryRequestOptions() { }
                );
                Console.WriteLine("Outside loop");

                //var results = new List<BudgetLineItemModel>();

                while (iterator.HasMoreResults)
                {
                    using (ResponseMessage response = await iterator.ReadNextAsync())
                    {
                        using (StreamReader sr = new StreamReader(response.Content))
                        using (JsonTextReader jtr = new JsonTextReader(sr))
                        {
                            Console.WriteLine("I'm here");
                            JObject result = JObject.Load(jtr);
                            Console.WriteLine(result);
                        }
                    }
                }
            }
            catch(Exception ex) //when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse.
                Console.WriteLine(ex.Message);
            }

            return new OkObjectResult("Success!");
        }
    }
}
