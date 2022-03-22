using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using Azure.Cosmos;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

const string EndpointUrl = "https://personal-finance-db.documents.azure.com:443/";
const string AuthorizationKey = "uKehVT4myAIG69BAYyLZOzHlxLh4Wx0JotaD0OQeg54lrcsWR8vQLpkAnfIKCv0j6Cd5hSCco26oyD9pQFbgwA==";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton(sp => new CosmosClient(EndpointUrl, AuthorizationKey));

await builder.Build().RunAsync();

CosmosClient cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey);
//await Budget.AddItemsToContainerAsync(cosmosClient);
