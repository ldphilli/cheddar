using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Cheddar.Client.Services {
    public class AuthenticationService {
    [Parameter]
    public string? Action { get; set; }
    IAccessTokenProvider? TokenProvider { get; set; }

    public string? AccessToken { get; set; }
    private ApplicationState appState;
    private readonly NavigationManager nvm;

    public AuthenticationService(NavigationManager navManager, ApplicationState applicationState, IAccessTokenProvider tp) {
        nvm = navManager;
        appState = applicationState;
        TokenProvider = tp;
    }

    // can't get navigate working here
    public async Task OnLoginSucceeded() {
        var accessTokenResult = await TokenProvider.RequestAccessToken();

            if (accessTokenResult.TryGetToken(out var token))
            {
                AccessToken = token.Value;
                //Console.WriteLine(AccessToken);
                appState.Token = AccessToken;
                //appState.Token = AccessToken;
                Console.WriteLine(appState.Token);
            } else {
                Console.WriteLine("No token");
            }
        nvm.NavigateTo("budget");
    }

        public void OnLogoutSucceeded() {
            nvm.NavigateTo("/");
        }
    }
}