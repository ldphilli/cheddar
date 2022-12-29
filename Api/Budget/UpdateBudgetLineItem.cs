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
  public static class UpdateBudgetLineItem
  {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        [FunctionName("UpdateBudgetLineItem")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetLineItemsContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
        ILogger log) {

            try {

                Console.WriteLine("\n1.6 - Patching a item using its Id");

                if (!req.Headers.TryGetValue("Authorization", out var token))
                {
                    return new BadRequestObjectResult("No token found");
                }

                Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.BudgetLineItemsContainerName);

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var item = JsonConvert.DeserializeObject<BudgetLineItemModel>(requestBody);

                IReadOnlyList<PatchOperation> patchOperations = new[] { 
                    PatchOperation.Replace("/BudgetLineName", item.BudgetLineName),
                    PatchOperation.Replace("/Cost", item.Cost),
                    PatchOperation.Replace("/Category", item.Category),
                    PatchOperation.Replace("/PaymentMethod", item.PaymentMethod),
                    PatchOperation.Replace("/ContractEndDate", item.ContractEndDate)
                };

                using (Stream stream = ToStream<IReadOnlyList<PatchOperation>>(patchOperations))
                {
                    using (ResponseMessage responseMessage = await container.PatchItemStreamAsync(
                        id: item.Id,
                        partitionKey: new PartitionKey(item.UserId),
                        patchOperations: patchOperations))
                    {
                        // Item stream operations do not throw exceptions for better performance
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            BudgetLineItemModel streamResponse = FromStream<BudgetLineItemModel>(responseMessage.Content);
                            Console.WriteLine($"\n1.6.3 - Item Patch via stream {streamResponse.Id}");
                        }
                        else
                        {
                            Console.WriteLine($"Patch item from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                        }
                    }
                }
                return new OkObjectResult("Success!");
            }
            catch(CosmosException cosmosException) {
                return new BadRequestObjectResult(cosmosException);
            }
        }

        private static Stream ToStream<T>(T input)
        {
            MemoryStream streamPayload = new MemoryStream();
            using (StreamWriter streamWriter = new StreamWriter(streamPayload, encoding: Encoding.Default, bufferSize: 1024, leaveOpen: true))
            {
                using (JsonWriter writer = new JsonTextWriter(streamWriter))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.None;
                    Serializer.Serialize(writer, input);
                    writer.Flush();
                    streamWriter.Flush();
                }
            }

            streamPayload.Position = 0;
            return streamPayload;
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