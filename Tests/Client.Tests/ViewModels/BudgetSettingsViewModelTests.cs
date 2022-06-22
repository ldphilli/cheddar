using Bunit;
using Bunit.TestDoubles;
using Cheddar.Client.Services;
using Cheddar.Client.ViewModels;
using Cheddar.Shared.Models;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Client.Tests;

public class BudgetSettingsViewModelTests
{
  private TestContext context; 
  private readonly FakeNavigationManager navigationManager;
  private readonly Mock<IBudgetSettingsService> budgetSettingsServiceMock;
  private readonly ApplicationState applicationState;
  private readonly BudgetSettingsViewModel systemUnderTest;

  public BudgetSettingsViewModelTests()
  {
    context = new TestContext();
    budgetSettingsServiceMock = new Mock<IBudgetSettingsService>();
    navigationManager = new FakeNavigationManager(context.Renderer);
    applicationState = new ApplicationState();

    systemUnderTest = new BudgetSettingsViewModel(
      navigationManager,
      applicationState,
      budgetSettingsServiceMock.Object
    );
  }


  [Fact]
  public void GivenConstructed_ThenModelsShouldNotBeNull()
  {
    // Assert
    systemUnderTest.budgetCategoryModel.Should().NotBeNull();
    systemUnderTest.budgetCategoryModel.Id.Should().NotBeNullOrEmpty();

    systemUnderTest.paymentMethodModel.Should().NotBeNull();
    systemUnderTest.paymentMethodModel.Id.Should().NotBeNullOrEmpty();

    systemUnderTest.budgetSettingsModel.Should().Be(applicationState.budgetSettingsModel);
    systemUnderTest.budgetSettingsModel.Id.Should().NotBeNullOrEmpty();

    systemUnderTest.remainingExpenditureCategoriesModel.Should().NotBeNull();
    systemUnderTest.remainingExpenditureCategoriesModel.Id.Should().NotBeNullOrEmpty();
  }

  [Fact]
  public async Task GivenAddBudgetCategoryToContainerAsyncCalled_ThenServiceCalledAndNavigateBack()
  {
    // Arrange
    var budgetCategoryModel = new BudgetCategoriesModel();

    // Act
    await systemUnderTest.AddBudgetCategoryToContainerAsync(budgetCategoryModel);

    // Assert
    budgetSettingsServiceMock.Verify(x => x.AddBudgetCategoryToContainerAsync(budgetCategoryModel));
    navigationManager.Uri.Should().Be("http://localhost/budget");
  }

  [Fact]
  public async Task GivenAddPaymentMethodToContainerAsyncCalled_ThenServiceCalledAndNavigateBack()
  {
    // Arrange
    var paymentMethodsModel = new PaymentMethodsModel();

    // Act
    await systemUnderTest.AddPaymentMethodToContainerAsync(paymentMethodsModel);

    // Assert
    budgetSettingsServiceMock.Verify(x => x.AddPaymentMethodToContainerAsync(paymentMethodsModel));
    navigationManager.Uri.Should().Be("http://localhost/budget");
  }

  [Fact]
  public async Task GivenGetBudgetSettingDataForUserCalled_ThenUpdateAppStateFromService()
  {
    // Arrange
    var budgetSettingsModel = new BudgetSettingsModel();

    budgetSettingsServiceMock
      .Setup(x => x.GetMonthlyIncome())
      .ReturnsAsync(budgetSettingsModel);

    // Act
    await systemUnderTest.GetBudgetSettingDataForUser();

    // Assert
    applicationState.budgetSettingsModel.Should().Be(budgetSettingsModel);
  }

  [Fact]
  public async Task GivenCreateOrUpdateMonthlyIncomeCalled_ThenServiceCalledAppStateUpdatedAndNavigateBack()
  {
    // Arrange
    var budgetSettingsModelOrginal = applicationState.budgetSettingsModel;
    var budgetSettingsModelNew = new BudgetSettingsModel();

    budgetSettingsServiceMock
      .Setup(x => x.GetMonthlyIncome())
      .ReturnsAsync(budgetSettingsModelNew);

    // Act
    await systemUnderTest.CreateOrUpdateMonthlyIncome();

    // Assert
    budgetSettingsServiceMock.Verify(x => x.CreateOrUpdateBudgetSettingsDoc(budgetSettingsModelOrginal));
    applicationState.budgetSettingsModel.Should().Be(budgetSettingsModelNew);
    navigationManager.Uri.Should().Be("http://localhost/budget");
  }

  [Fact]
  public async Task GivenCreateRemainingExpenditureCategoryCalled_ThenServiceCalledAndNavigateBack()
  {
    // Act
    await systemUnderTest.CreateRemainingExpenditureCategory();

    // Assert
    budgetSettingsServiceMock.Verify(x => x.CreateRemainingExpenditureCategoriesDoc(systemUnderTest.remainingExpenditureCategoriesModel));
    navigationManager.Uri.Should().Be("http://localhost/budget");
  }

}