using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using Cheddar.Client.Services;
using Cheddar.Client.Shared;
using Cheddar.Client.ViewModels;
using Cheddar.Shared.Models;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) });

// builder.Services
//     .AddHttpClient<BudgetSettingsService>(client => client.BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress))
//     .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
//     .ConfigureHandler(authorizedUrls: new [] { 
//         builder.Configuration["API_Prefix"]
//         }));

builder.Services.AddHttpClient("WebAPI", 
        client => client.BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
     .ConfigureHandler(authorizedUrls: new [] { 
         builder.Configuration["API_Prefix"]
         }));
         
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("WebAPI"))
    .AddSingleton<ApplicationState>();

builder.Services.AddTransient<BudgetSettingsService>();
builder.Services.AddTransient<AuthenticationService>();
builder.Services.AddTransient<SalaryUpdateViewModel>();
builder.Services.AddTransient<BudgetLineItemVM>();
builder.Services.AddTransient<BudgetSettingsViewModel>();
builder.Services.AddTransient<BudgetVM>();
builder.Services.AddTransient<WelcomeViewModel>();
builder.Services.AddTransient<AuthenticationService>();
//Add application state model as a singleton
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
