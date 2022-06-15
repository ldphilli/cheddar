using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Cheddar.Client.Services
{
    public class AuthenticationService
    {
        IAccessTokenProvider? TokenProvider { get; set; }

        private ApplicationState appState;
        private readonly NavigationManager nvm;

        public AuthenticationService(NavigationManager navManager, ApplicationState applicationState, IAccessTokenProvider tp)
        {
            nvm = navManager;
            appState = applicationState;
            TokenProvider = tp;
        }

        // can't get navigate working here
        public async Task OnLoginSucceeded()
        {
            var accessTokenResult = await TokenProvider.RequestAccessToken();

            if (accessTokenResult.TryGetToken(out var token))
            {
                appState.Token = token.Value;
            }
            else
            {
                Console.WriteLine("No token");
            }
            nvm.NavigateTo("budget");
        }

        public void OnLogoutSucceeded()
        {
            nvm.NavigateTo("/");
        }
    }
}