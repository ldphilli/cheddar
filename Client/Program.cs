using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using Cheddar.Client.Services;
using Cheddar.Client.ViewModels;
using Cheddar.Shared.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress);
//Console.WriteLine("Base Address: " + baseAddress);

builder.Services
    .AddHttpClient("WebAPI", client => client.BaseAddress = baseAddress)
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>().ConfigureHandler(authorizedUrls: new[] { baseAddress.ToString() }));

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebAPI"));
builder.Services.AddTransient<BudgetSettingsService>();
builder.Services.AddTransient<SalaryUpdateViewModel>();
builder.Services.AddTransient<BudgetLineItemViewModel>();
builder.Services.AddTransient<BudgetSettingsViewModel>();
builder.Services.AddTransient<WelcomeViewModel>();
builder.Services.AddTransient<BudgetViewModel>();
builder.Services.AddSingleton<ApplicationState>();
builder.Services.AddMsalAuthentication(options =>
{
  builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
  //options.ProviderOptions.DefaultAccessTokenScopes.Add("https://CheddarApp.onmicrosoft.com/25b6b977-0a24-4ba7-9795-45437c5cff10/Api.ReadWrite");
  options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
  options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
  options.ProviderOptions.DefaultAccessTokenScopes.Add("e740fe9f-b317-4aa5-8196-9be58e0b30e3");
  options.AuthenticationPaths.LogOutSucceededPath = "/";
});

await builder.Build().RunAsync();
