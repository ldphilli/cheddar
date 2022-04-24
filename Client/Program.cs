using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using Cheddar.Client.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) });
builder.Services.AddTransient<SalaryUpdateViewModel>();
builder.Services.AddTransient<BudgetLineItemVM>();
builder.Services.AddTransient<BudgetSettingsViewModel>();
builder.Services.AddTransient<BudgetVM>();

await builder.Build().RunAsync();
