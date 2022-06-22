using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using Cheddar.Client.Services;

namespace Cheddar.Client.ViewModels
{
    public class BudgetSettingsViewModel
    {
        private ApplicationState appState;
        public BudgetCategoriesModel budgetCategoryModel { get; set; }
        public PaymentMethodsModel paymentMethodModel { get; set; }
        public RemainingExpenditureCategoriesModel remainingExpenditureCategoriesModel { get; set; }
        public BudgetSettingsModel budgetSettingsModel => appState.budgetSettingsModel;
        public IBudgetSettingsService budgetSettingsService;

        private readonly NavigationManager nvm;

        public BudgetSettingsViewModel(NavigationManager navManager, ApplicationState applicationState, IBudgetSettingsService bsService)
        {
            nvm = navManager;
            appState = applicationState;
            budgetSettingsService = bsService;

            budgetCategoryModel = new BudgetCategoriesModel();
            budgetCategoryModel.Id = Guid.NewGuid().ToString();

            paymentMethodModel = new PaymentMethodsModel();
            paymentMethodModel.Id = Guid.NewGuid().ToString();

            appState.budgetSettingsModel = new BudgetSettingsModel();
            budgetSettingsModel.Id = Guid.NewGuid().ToString();

            remainingExpenditureCategoriesModel = new RemainingExpenditureCategoriesModel();
            remainingExpenditureCategoriesModel.Id = Guid.NewGuid().ToString();
        }

        public async Task AddBudgetCategoryToContainerAsync(BudgetCategoriesModel budgetCategory)
        {
            await budgetSettingsService.AddBudgetCategoryToContainerAsync(budgetCategory);
            nvm.NavigateTo("/budget");
        }

        public async Task AddPaymentMethodToContainerAsync(PaymentMethodsModel paymentMethod)
        {
            await budgetSettingsService.AddPaymentMethodToContainerAsync(paymentMethod);
            nvm.NavigateTo("/budget");
        }

        public async Task GetBudgetSettingDataForUser()
        {
            appState.budgetSettingsModel = await budgetSettingsService.GetMonthlyIncome();
        }

        public async Task CreateOrUpdateMonthlyIncome()
        {
            await budgetSettingsService.CreateOrUpdateBudgetSettingsDoc(budgetSettingsModel);
            await GetBudgetSettingDataForUser();
            nvm.NavigateTo("/budget");
        }

        public async Task CreateRemainingExpenditureCategory()
        {
            await budgetSettingsService.CreateRemainingExpenditureCategoriesDoc(remainingExpenditureCategoriesModel);
            nvm.NavigateTo("/budget");
        }
    }
}