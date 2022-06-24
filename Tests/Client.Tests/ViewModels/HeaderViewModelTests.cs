using Cheddar.Client.Services;
using Cheddar.Client.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using System;
using System.Security.Claims;
using Xunit;

namespace Client.Tests;

public class HeaderViewModelTests
{
  [Theory]
  [InlineData(1)]
  [InlineData(6)]
  [InlineData(11)]
  public void GivenCurrentHourIsLessThan12_ThenGoodMorningShouldBeReturned(int hour)
  {
    // Arrange
    var clockServiceMock = new Mock<IClockService>();
    clockServiceMock.Setup(x => x.Now).Returns(new DateTime(2001, 1, 1, hour, 0, 0));
    var authenticationStateProviderMock = new Mock<AuthenticationStateProvider>();

    // Act
    var systemUnderTest = new HeaderViewModel(clockServiceMock.Object, authenticationStateProviderMock.Object);

    // Assert
    systemUnderTest.timeOfDayAsText.Should().Be("Good morning");
  }

  [Theory]
  [InlineData(12)]
  [InlineData(15)]
  [InlineData(17)]
  public void GivenCurrentHourIsMoreThan11AndLessThan18_ThenGoodAfternoonShouldBeReturned(int hour)
  {
    // Arrange
    var clockServiceMock = new Mock<IClockService>();
    clockServiceMock.Setup(x => x.Now).Returns(new DateTime(2001, 1, 1, hour, 0, 0));
    var authenticationStateProviderMock = new Mock<AuthenticationStateProvider>();

    // Act
    var systemUnderTest = new HeaderViewModel(clockServiceMock.Object, authenticationStateProviderMock.Object);

    // Assert
    systemUnderTest.timeOfDayAsText.Should().Be("Good afternoon");
  }

  [Theory]
  [InlineData(18)]
  [InlineData(21)]
  [InlineData(23)]
  [InlineData(0)]
  public void GivenCurrentHourIsMoreThan17AndLessThan1_ThenGoodEveningShouldBeReturned(int hour)
  {
    // Arrange
    var clockServiceMock = new Mock<IClockService>();
    clockServiceMock.Setup(x => x.Now).Returns(new DateTime(2001, 1, 1, hour, 0, 0));
    var authenticationStateProviderMock = new Mock<AuthenticationStateProvider>();

    // Act
    var systemUnderTest = new HeaderViewModel(clockServiceMock.Object, authenticationStateProviderMock.Object);

    // Assert
    systemUnderTest.timeOfDayAsText.Should().Be("Good evening");
  }

  [Fact]
  public void GivenGivenNameAvailableInClaims_ThenGivenNameShouldBeReturned()
  {
    // Arrange
    var clockServiceMock = new Mock<IClockService>();

    var username = "UserName";
    var authenticationStateProviderMock = new Mock<AuthenticationStateProvider>();
    authenticationStateProviderMock
      .Setup(x => x.GetAuthenticationStateAsync())
      .ReturnsAsync(new AuthenticationState(new ClaimsPrincipal(new[]{
        new ClaimsIdentity(new [] {
          new Claim("given_name", username)
        })
      })));

    // Act
    var systemUnderTest = new HeaderViewModel(clockServiceMock.Object, authenticationStateProviderMock.Object);

    // Assert
    systemUnderTest.userName.Should().Be(username);
  }
}