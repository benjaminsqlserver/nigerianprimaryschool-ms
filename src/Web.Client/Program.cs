using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Auth state reads persisted state from the server component
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<
    Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider,
    NigerianPrimarySchool.Web.Client.PersistentAuthenticationStateProvider>();

// Radzen services (dialog, notification, etc.)
builder.Services.AddRadzenComponents();

await builder.Build().RunAsync();
