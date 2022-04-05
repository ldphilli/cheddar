using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private static readonly JsonSerializer Serializer = new JsonSerializer();
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
                List<BudgetLineItemModel> budgetLineItems = new List<BudgetLineItemModel>();
                //IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
                var queryDefinition = new QueryDefinition("SELECT * FROM c where c.UserId = @userId")
                .WithParameter("@userId", 1);
                /*FeedIterator iterator = container.GetItemQueryStreamIterator(
                    feedRanges[0],
                    queryDefinition,
                    null,
                    new QueryRequestOptions()
                );

                JObject result = new JObject();
                while (iterator.HasMoreResults)
                {
                    using (ResponseMessage response = await iterator.ReadNextAsync())
                    {
                        using (StreamReader sr = new StreamReader(response.Content))
                        using (JsonTextReader jtr = new JsonTextReader(sr))
                        {
                            result = JObject.Load(jtr);
                            Console.WriteLine(result.ToString());
                            var jsonString = JsonConvert.SerializeObject(result.ToString());
                            Console.WriteLine(jsonString);
                            BudgetLineItemModel budgetModel = JsonConvert.DeserializeObject<BudgetLineItemModel>(jsonString);
                            budgetLineItems.Add(budgetModel);
                        }
                    }
                }

                /*FeedIterator<BudgetLineItemModel> resultSet = container.GetItemQueryIterator<BudgetLineItemModel>(
                    queryDefinition,
                    requestOptions: new QueryRequestOptions()
                    {
                        PartitionKey = new PartitionKey("UserId")
                    }
                );

                while(resultSet.HasMoreResults)
                {
                    FeedResponse<BudgetLineItemModel> response = await resultSet.ReadNextAsync();
                    BudgetLineItemModel budgetLineItem = response.First();
                    Console.WriteLine($"\n1.3.1 Account Number: {budgetLineItem.BudgetLineName}; Id: {budgetLineItem.Id};");
                    if (response.Diagnostics != null)
                    {
                        Console.WriteLine($" Diagnostics {response.Diagnostics.ToString()}");
                    }

                    budgetLineItems.AddRange(response);
                }
                Console.WriteLine($"\n1.3.2 Read all items found {budgetLineItems.Count} items.");*/
                List<BudgetLineItemModel> allBudgetLineItemsForUser = new List<BudgetLineItemModel>();
                using (FeedIterator streamResultSet = container.GetItemQueryStreamIterator(
                    queryDefinition,
                    null,
                    new QueryRequestOptions()
                ))

                while (streamResultSet.HasMoreResults)
                {
                    using (ResponseMessage responseMessage = await streamResultSet.ReadNextAsync())
                    {
                        // Item stream operations do not throw exceptions for better performance
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            dynamic streamResponse = FromStream<dynamic>(responseMessage.Content);
                            List<BudgetLineItemModel> salesOrders = streamResponse.Documents.ToObject<List<BudgetLineItemModel>>();
                            Console.WriteLine($"\n1.3.3 - Read all items via stream {salesOrders.Count}");
                            budgetLineItems.AddRange(salesOrders);
                            Console.WriteLine(salesOrders.First().BudgetLineName);
                        }
                        else
                        {
                            Console.WriteLine($"Read all items from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                        }
                    }
                }

                Console.WriteLine($"\n1.3.4 Read all items found {allBudgetLineItemsForUser.Count} items.");

                if (budgetLineItems.Count != allBudgetLineItemsForUser.Count)
                {
                    throw new InvalidDataException($"Both read all item operations should return the same list");
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

        private static T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)stream;
                }

                using (StreamReader sr = new StreamReader(stream))
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                    {
                        return Serializer.Deserialize<T>(jsonTextReader);
                    }
                }
            }
        }
    }
}
