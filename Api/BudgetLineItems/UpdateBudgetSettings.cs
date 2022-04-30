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
  public static class UpdateBudgetSettings
  {

        private static readonly JsonSerializer Serializer = new JsonSerializer();

        [FunctionName("UpdateBudgetSettings")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        [CosmosDB(
                databaseName: DbConfiguration.DBName,
                containerName: DbConfiguration.BudgetSettingsContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
        ILogger log) {

            Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.BudgetSettingsContainerName);

            try {
                //IBudgetSettingsModel salesOrderV4 = //GetSalesOrderSample("SalesOrder4");

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var item = JsonConvert.DeserializeObject<IBudgetSettingsModel>(requestBody);
                log.LogInformation(item.UserId.ToString());
                using (Stream stream = ToStream<IBudgetSettingsModel>(item))
                {
                    using (ResponseMessage responseMessage = await container.UpsertItemStreamAsync(
                        partitionKey: new PartitionKey("UserId"),
                        streamPayload: stream))
                    {
                        // Item stream operations do not throw exceptions for better performance
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            IBudgetSettingsModel streamResponse = FromStream<IBudgetSettingsModel>(responseMessage.Content);
                            Console.WriteLine($"\n1.7.2 - Item upserted via stream {streamResponse.Id}");
                        }
                        else
                        {
                            Console.WriteLine($"Upsert item from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
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