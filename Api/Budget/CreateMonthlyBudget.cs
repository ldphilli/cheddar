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
  public static class CreateMonthlyBudget
  {
    private static readonly JsonSerializer Serializer = new JsonSerializer();

    [FunctionName("CreateMonthlyBudget")]
    public static async Task Run(
      [TimerTrigger("0 0 2 * * *") /* Every day at 2AM */] TimerInfo myTimer,
      [CosmosDB(
        databaseName: DbConfiguration.DBName,
        containerName: DbConfiguration.MonthlyBudgetContainerName,
        Connection = "CosmosDBConnection")] IAsyncCollector<MonthlyBudgetModel> documentsOut,
      [CosmosDB(
        databaseName: DbConfiguration.DBName,
        containerName: DbConfiguration.BudgetSettingsContainerName,
        Connection = "CosmosDBConnection")] CosmosClient client,
      ILogger log)
    {

      if (myTimer.IsPastDue)
      {
        log.LogInformation("Timer is running late!");
      }

      log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

      //Get list of users from budget settings where monthlybudgetdate = today
      Container budgetSettingsContainer = client.GetContainer(DbConfiguration.DBName, DbConfiguration.BudgetSettingsContainerName);
      Container budgetLineItemsContainer = client.GetContainer(DbConfiguration.DBName, DbConfiguration.BudgetLineItemsContainerName);
      Container remainingExpenditureCategoriesContainer = client.GetContainer(DbConfiguration.DBName, DbConfiguration.RemainingExpenditureCategoriesContainerName);

      try
      {
        var allUsersWhoNeedNewBudgetToday = budgetSettingsContainer
            .GetItemLinqQueryable<BudgetSettingsModel>(true)
            .Where(x => x.MonthlyBudgetDay == DateTimeOffset.Now.Day)
            .AsEnumerable();

        foreach (var budgetSetting in allUsersWhoNeedNewBudgetToday)
        {
          await CreateMonthlyBudgetForUser(
            budgetSetting,
            budgetLineItemsContainer,
            remainingExpenditureCategoriesContainer,
            documentsOut,
            log);
        }
      }
      catch (CosmosException cosmosException)
      {
        log.LogError(cosmosException.ToString());
      }
    }

    private static async Task CreateMonthlyBudgetForUser(
      BudgetSettingsModel budgetSetting,
      Container budgetLineItemsContainer,
      Container remainingExpenditureCategoriesContainer,
      IAsyncCollector<MonthlyBudgetModel> documentsOut,
      ILogger log)
    {
      var userId = budgetSetting.userId;
     
      log.LogInformation($"Creating monthly budget for user '{userId}' using settings '{budgetSetting.Id}");

      if (userId == null || userId == string.Empty)
      {
        log.LogInformation($"User id for budget settings '{budgetSetting.Id}' was null/empty");
        return;
      }

      var budgetLineItemsForUser = await GetBudgetLineItems.GetBudgetLineItemData(budgetLineItemsContainer, userId);
      var remainingExpenditureCategoriesForUser = await GetRemainingExpenditureCategories.GetRemainingExpenditureCategoryData(remainingExpenditureCategoriesContainer, userId);

      if (!budgetLineItemsForUser.Any())
      {
        log.LogInformation("Budget line items empty");
        return;
      }

      log.LogInformation("Budget line items found");
      var totalCostOfExpenses = budgetLineItemsForUser.Sum(x => x.Cost);
      var remainingBudget = Math.Round(budgetSetting.MonthlyIncome - totalCostOfExpenses, 2);

      // Create and establish values for monthly budget
      MonthlyBudgetModel monthlyBudget = new MonthlyBudgetModel
      {
        Id = Guid.NewGuid().ToString(),
        UserId = userId,
        Remaining = remainingBudget,
        Income = budgetSetting.MonthlyIncome,
        Outgoing = totalCostOfExpenses,
        Year = DateTime.Now.Year,
        Month = DateTime.Now.Month,
        expenditureCategories = remainingExpenditureCategoriesForUser
          .Select(x => new RemainingExpenditureCategoriesWithAmountModel
          {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            CategoryName = x.CategoryName,
            Amount = Math.Round(remainingBudget * ((double)x.Percentage / (double)100), 2)
          })
          .ToList(),
        BudgetLineItemsForMonth = budgetLineItemsForUser
        .Select(x => new BudgetLineItemModel
        {
          Id = Guid.NewGuid().ToString(),
          UserId = x.UserId,
          BudgetLineName = x.BudgetLineName,
          Category = x.Category,
          ContractEndDate = x.ContractEndDate,
          Cost = x.Cost,
          PaymentMethod = x.PaymentMethod
        })
        .ToList()
      };

      //Create monthly budget for user
      await documentsOut.AddAsync(monthlyBudget);
      log.LogInformation("Monthly budget for user created");
    }
  }
}
