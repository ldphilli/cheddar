using Microsoft.AspNetCore.Components;

namespace Cheddar.Tests
{
  public class FakeNavigationManager : NavigationManager
  {
    public FakeNavigationManager() {
      Initialize("http://localhost/", "http://localhost/");
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
      this.Uri = ToAbsoluteUri(uri).OriginalString;
    }

    protected override void NavigateToCore(string uri, NavigationOptions options)
    {
      this.Uri = ToAbsoluteUri(uri).OriginalString;
    }
  }
}