using Cheddar.Shared.Models;

namespace Cheddar.Client.Services.Interfaces
{
  public interface IBudgetSettingsService
  {
    Task<BudgetSettingsModel?> GetMonthlyIncome();

    Task CreateOrUpdateBudgetSettingsDoc(BudgetSettingsModel budgetSettings);

    Task CreateRemainingExpenditureCategoriesDoc(RemainingExpenditureCategoriesModel remainingExpenditureCategory);

    Task AddBudgetCategoryToContainerAsync(BudgetCategoriesModel budgetCategory);

    Task AddPaymentMethodToContainerAsync(PaymentMethodsModel paymentMethod);
  }
}
