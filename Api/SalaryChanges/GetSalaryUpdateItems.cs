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
    public static class GetSalaryUpdateItems {
        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private const string EndpointUrl = "https://personal-finance-db.documents.azure.com:443/";
        private const string AuthorizationKey = "uKehVT4myAIG69BAYyLZOzHlxLh4Wx0JotaD0OQeg54lrcsWR8vQLpkAnfIKCv0j6Cd5hSCco26oyD9pQFbgwA==";
        [FunctionName("GetSalaryUpdateItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: IDBOptionsModel.DBName,
                containerName: IDBOptionsModel.SalaryUpdateItemsContainerName,
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log) {
            
            Container container = client.GetContainer(IDBOptionsModel.DBName, IDBOptionsModel.SalaryUpdateItemsContainerName);

            try {
                List<ISalaryUpdateModel> allSalaryUpdateItemsForUser = new List<ISalaryUpdateModel>();

                //Setup query to database, get all budget line items for current user
                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c where c.UserId = @userId")
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
                            List<ISalaryUpdateModel> salaryUpdateItems = streamResponse.Documents.ToObject<List<ISalaryUpdateModel>>();
                            allSalaryUpdateItemsForUser.AddRange(salaryUpdateItems);
                        }
                        //If no results are returned
                        else {
                            Console.WriteLine($"Read all items from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                        }
                    }
                }
                return new OkObjectResult(allSalaryUpdateItemsForUser);
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
