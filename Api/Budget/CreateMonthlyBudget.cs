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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cheddar.Api.Configuration;

namespace Cheddar.Function
{
  public static class CreateMonthlyBudget {

    private static readonly JsonSerializer Serializer = new JsonSerializer();
    private static jwtManagementToken manageToken = new jwtManagementToken();

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
                    string userId = manageToken.GetUserIdFromToken("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL2NoZWRkYXJhcHAuYjJjbG9naW4uY29tLzlhMmQ5NWVmLTQwYjgtNDAwNC1iMWFhLWIyODYwZGM0NGIxYi92Mi4wLyIsImV4cCI6MTY1MzM4Mzk2NSwibmJmIjoxNjUzMzgwMzY1LCJhdWQiOiJlNzQwZmU5Zi1iMzE3LTRhYTUtODE5Ni05YmU1OGUwYjMwZTMiLCJzdWIiOiI1YWVjYjk0YS1hYzRjLTQ2ZWYtYTFlZC02N2E1YWQyMTBmNDciLCJnaXZlbl9uYW1lIjoiTHVrZSIsInRmcCI6IkIyQ18xX0NoZWRkYXJTaWduSW5TaWduVXAiLCJub25jZSI6IjYzNzQzYzZhLTYxNDAtNDY4MC1iZGEwLTNkZDI2ZWRhNTE1YiIsImF6cCI6ImU3NDBmZTlmLWIzMTctNGFhNS04MTk2LTliZTU4ZTBiMzBlMyIsInZlciI6IjEuMCIsImlhdCI6MTY1MzM4MDM2NX0.ZARNA6snDebC-MyiQpK5ZcbvK44u6HvlQLQAR0lWdEYD98k4Z_j_ewpSVZza0MM0WZDFPi8x4_53CZ5U7wKInqCBjj6LkYGSXCJW2zMxgY5sEP6A_3dJFgLY4Xk7rbfwewjJluWEhUEseuzY0Wh7TJfCDq1xrGRYZPp_IlYFJ_xjFbSmfWM7Mmrb3ZWDBl-NthR8aeFhoOkt9DG2qKeI72DlxNv1AzujGIbr9weeXmP_AiuLi9QcTSbERzwfHudmeQfZA2C_GvTd-Qs2c5LeI88ZFisSkXOVPd7UyoLq4ctTzQ2lOT32ZirE_pmN3-oSMoHe1PfJe7uU2BvGLlPAOg");
                    if(userId != null || userId != string.Empty) {
                        monthlyBudget.UserId = userId;
                    }
                    monthlyBudget.Remaining = budgetSetting.MonthlyIncome - totalCostOfExpenses;
                    monthlyBudget.Income = budgetSetting.MonthlyIncome;
                    monthlyBudget.Outgoing = totalCostOfExpenses;
                    monthlyBudget.Year = DateTime.Now.Year;
                    monthlyBudget.Month = DateTime.Now.Month;
                    remainingExpenditureCategoriesForUser = await GetRemainingExpenditureCategories.GetRemainingExpenditureCategoryData(remainingExpenditureCategoriesContainer);
                    foreach(var expenditureCategory in remainingExpenditureCategoriesForUser) {
                        RemainingExpenditureCategoriesWithAmountModel remExpenditureCategory = new RemainingExpenditureCategoriesWithAmountModel();
                        remExpenditureCategory.Id = Guid.NewGuid().ToString();
                        remExpenditureCategory.UserId = userId;
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