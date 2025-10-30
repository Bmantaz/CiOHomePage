using CiOHomePage.Client;
using CiOHomePage.Client.Services;
using CiOHomePage.Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Mount root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Read API base from config (fallback to client origin)
var apiBase = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;

// Default HttpClient for static assets (e.g., sample-data/*.json)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Auth handler for API calls
builder.Services.AddTransient<AuthMessageHandler>();

// Typed HttpClients for API-bound services
builder.Services.AddHttpClient<JwtBandAuthService>(client =>
{
 client.BaseAddress = new Uri(apiBase);
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<HttpCalendarService>(client =>
{
 client.BaseAddress = new Uri(apiBase);
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<MerchService>(client =>
{
 client.BaseAddress = new Uri(apiBase);
})
.AddHttpMessageHandler<AuthMessageHandler>();

// Gig service uses API base and includes auth for POST endpoints
builder.Services.AddHttpClient<IGigService, GigService>(client =>
{
 client.BaseAddress = new Uri(apiBase);
})
.AddHttpMessageHandler<AuthMessageHandler>();

// AuthN/AuthZ
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, BandAuthStateProvider>();
builder.Services.AddScoped<IBandAuthService>(sp => sp.GetRequiredService<JwtBandAuthService>());

// Local storage
builder.Services.AddScoped<ILocalStorage, BrowserLocalStorage>();

// App services
builder.Services.AddScoped<ICalendarService>(sp => sp.GetRequiredService<HttpCalendarService>());
builder.Services.AddScoped<IMerchService>(sp => sp.GetRequiredService<MerchService>());

await builder.Build().RunAsync();
