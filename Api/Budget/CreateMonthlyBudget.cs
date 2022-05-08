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
using System.Linq;
using System.Threading.Tasks;
using Cheddar.Api.Configuration;

namespace Cheddar.Function
{
  public static class CreateMonthlyBudget {

    private static readonly JsonSerializer Serializer = new JsonSerializer();

    [FunctionName("CreateMonthlyBudget")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, 
    [CosmosDB(
        databaseName: DbConfiguration.DBName,
        containerName: DbConfiguration.MonthlyBudgetContainerNamer,
        Connection = "CosmosDBConnection")] IAsyncCollector<MonthlyBudgetModel> documentsOut, 
        [CosmosDB(
        databaseName: DbConfiguration.DBName,
        containerName: DbConfiguration.BudgetSettingsContainerName,
        Connection = "CosmosDBConnection")] CosmosClient client,
        ILogger log) {

        /*if (myTimer.IsPastDue)
        {
            log.LogInformation("Timer is running late!");
        }*/
        
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        //Get list of users from budget settings where monthlybudgetdate = today
        Container container = client.GetContainer(DbConfiguration.DBName, DbConfiguration.BudgetSettingsContainerName);
        Container budgetLineItemsContainer = client.GetContainer(DbConfiguration.DBName, DbConfiguration.BudgetLineItemsContainerName);
        Container remainingExpenditureCategoriesContainer = client.GetContainer(DbConfiguration.DBName, DbConfiguration.RemainingExpenditureCategoriesContainerName);

        try {
            List<BudgetSettingsModel> allUsersWhoNeedNewBudgetToday = new List<BudgetSettingsModel>();
            List<BudgetLineItemModel> budgetLineItemsForUser = new List<BudgetLineItemModel>();
            List<RemainingExpenditureCategoriesModel> remainingExpenditureCategoriesForUser = new List<RemainingExpenditureCategoriesModel>();
            List<RemainingExpenditureCategoriesWithAmountModel> expenditureCategoriesWithAmountForUser = new List<RemainingExpenditureCategoriesWithAmountModel>();
            double totalCostOfExpenses;
            //Setup query to database, get all budget line items for current user
            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c where c.MonthlyBudgetDay = DateTimePart('d', GetCurrentDateTime())");
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
                        List<BudgetSettingsModel> usersWhoNeedNewBudgetToday = streamResponse.Documents.ToObject<List<BudgetSettingsModel>>();
                        allUsersWhoNeedNewBudgetToday.AddRange(usersWhoNeedNewBudgetToday);
                        log.LogInformation(usersWhoNeedNewBudgetToday.First().Id);
                    }
                        //If no results are returned
                    else {
                        Console.WriteLine($"Read all items from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                    }
                }
            }

            
            foreach(var budgetSetting in allUsersWhoNeedNewBudgetToday) {
                // Call to get budget line items for user
                budgetLineItemsForUser = await GetBudgetLineItems.GetBudgetLineItemData(budgetLineItemsContainer);
                
                //Get remaining income - sum of budgetline items returned
                if(budgetLineItemsForUser.Any()) {
                    log.LogInformation("Budget line items found");
                    log.LogInformation(budgetLineItemsForUser.First().Cost.ToString());
                    totalCostOfExpenses = budgetLineItemsForUser.Sum(x => x.Cost);
                    log.LogInformation(totalCostOfExpenses.ToString());
                    MonthlyBudgetModel monthlyBudget = new MonthlyBudgetModel();
                    monthlyBudget.Id = Guid.NewGuid().ToString();
                    monthlyBudget.UserId = 2;
                    monthlyBudget.Remaining = budgetSetting.MonthlyIncome - totalCostOfExpenses;
                    monthlyBudget.Income = budgetSetting.MonthlyIncome;
                    monthlyBudget.Outgoing = totalCostOfExpenses;
                    monthlyBudget.Year = DateTime.Now.Year;
                    monthlyBudget.Month = DateTime.Now.Month;
                    remainingExpenditureCategoriesForUser = await GetRemainingExpenditureCategories.GetRemainingExpenditureCategoryData(remainingExpenditureCategoriesContainer);
                    foreach(var expenditureCategory in remainingExpenditureCategoriesForUser) {
                        RemainingExpenditureCategoriesWithAmountModel remExpenditureCategory = new RemainingExpenditureCategoriesWithAmountModel();
                        remExpenditureCategory.Id = Guid.NewGuid().ToString();
                        remExpenditureCategory.UserId = 2;
                        remExpenditureCategory.CategoryName = expenditureCategory.CategoryName;
                        remExpenditureCategory.Amount = monthlyBudget.Remaining * (expenditureCategory.Percentage / 100);
                        monthlyBudget.expenditureCategories.Add(remExpenditureCategory);
                    }
                    //Create monthly budget for user
                    await documentsOut.AddAsync(monthlyBudget);
                    log.LogInformation("Monthly budget for user created");
                }
                else {
                    log.LogInformation("Budget line items empty");
                }
            }
            return new OkObjectResult("Success");        
        }
        catch(CosmosException cosmosException) { //when (ex.Status == (int)HttpStatusCode.NotFound)
            return new BadRequestObjectResult("Bad");  
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