using Cheddar.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cheddar.Client.ViewModels
{
  public interface IIndexViewModel
  {
    Task RedirectToAppIfLoggedIn();
  }

  public class IndexViewModel : IIndexViewModel
  {
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly NavigationManager navigationManager;

    public IndexViewModel(AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager)
    {
      this.authenticationStateProvider = authenticationStateProvider;
      this.navigationManager = navigationManager;
    }

    public async Task RedirectToAppIfLoggedIn()
    {
      var authState = await authenticationStateProvider.GetAuthenticationStateAsync();

      if (authState?.User?.Identity?.IsAuthenticated ?? false) {
        navigationManager.NavigateTo("/budget");
      }
    }
  }
}