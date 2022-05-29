using Microsoft.AspNetCore.Components.Authorization;

namespace Cheddar.Client.Shared {
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync() {
            throw new NotImplementedException();
        }
    }
}