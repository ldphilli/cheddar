using Microsoft.AspNetCore.Components;

namespace Cheddar.Client.ViewModels {

    public class WelcomeViewModel {

        private readonly NavigationManager nvm;
    
        public void Login() {
            nvm.NavigateTo("authentication/login");
        }
    }
}