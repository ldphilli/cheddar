using Cheddar.Client.Services;
using Cheddar.Shared.Models;
using System.Net.Http.Json;

namespace Cheddar.Client.Services
{
  public interface IBudgetSettingsService
  {
    Task<BudgetSettingsModel?> GetMonthlyIncome();

    Task CreateOrUpdateBudgetSettingsDoc(BudgetSettingsModel budgetSettings);

    Task CreateRemainingExpenditureCategoriesDoc(RemainingExpenditureCategoriesModel remainingExpenditureCategory);

    Task AddBudgetCategoryToContainerAsync(BudgetCategoriesModel budgetCategory);

    Task AddPaymentMethodToContainerAsync(PaymentMethodsModel paymentMethod);
  }
  
  public class BudgetSettingsService : IBudgetSettingsService
  {

    private readonly HttpClient ApiClient;

    public BudgetSettingsService(HttpClient apiClient)
    {
      ApiClient = apiClient;
    }

    public async Task<BudgetSettingsModel?> GetMonthlyIncome()
    {
      return await ApiClient.GetFromJsonAsync<BudgetSettingsModel>("api/GetMonthlyIncome");
    }

    public async Task CreateOrUpdateBudgetSettingsDoc(BudgetSettingsModel budgetSettings)
    {
      await ApiClient.PostAsJsonAsync("api/CreateOrUpdateBudgetSettingsDoc", budgetSettings);
    }

    public async Task CreateRemainingExpenditureCategoriesDoc(RemainingExpenditureCategoriesModel remainingExpenditureCategory)
    {
      await ApiClient.PostAsJsonAsync("api/CreateRemainingExpenditureCategories", remainingExpenditureCategory);
    }

    public async Task AddBudgetCategoryToContainerAsync(BudgetCategoriesModel budgetCategory)
    {
      await ApiClient.PostAsJsonAsync("api/CreateBudgetCategory", budgetCategory);
    }

    public async Task AddPaymentMethodToContainerAsync(PaymentMethodsModel paymentMethod)
    {
      await ApiClient.PostAsJsonAsync("api/CreatePaymentMethod", paymentMethod);
    }
  }
}
