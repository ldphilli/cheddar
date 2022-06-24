using Cheddar.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cheddar.Client.ViewModels
{
  public interface IHeaderViewModel
  {
    string timeOfDayAsText { get; }

    string userName {get; }
  }

  public class HeaderViewModel : IHeaderViewModel
  {
    private readonly IClockService clock;
    private readonly AuthenticationStateProvider authenticationStateProvider;

    public string timeOfDayAsText { get; private set; } = "";
    public string userName { get; private set; } = "";

    public HeaderViewModel(IClockService clock, AuthenticationStateProvider authenticationStateProvider)
    {
      this.clock = clock;
      this.authenticationStateProvider = authenticationStateProvider;

      ConvertTimeOfDayToText();
      GetUserName();
    }

    private void ConvertTimeOfDayToText()
    {
      int hour = clock.Now.Hour;

      if (hour >= 1 && hour <= 11)
      {
        timeOfDayAsText = "Good morning";
      }
      else if (hour >= 12 && hour <= 17)
      {
        timeOfDayAsText = "Good afternoon";
      }
      else
      {
        timeOfDayAsText = "Good evening";
      }
    }

    private async Task GetUserName()
    {
      var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
      userName = authState?.User.FindFirst(x => x.Type == "given_name")?.Value ?? string.Empty;
    }
  }
}