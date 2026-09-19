using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TechLogistics.Client.Auth;
using TechLogistics.Client.Services;
using TechLogistics.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// HttpClient tipado para el cliente REST de catálogo/despachos, apuntando al mismo origen
// que sirve la aplicación (host Blazor Web App).
builder.Services.AddHttpClient<ICatalogApiClient, CatalogApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

// Servicio de persistencia local para el escenario offline-first del agente de campo.
builder.Services.AddScoped<ILocalDispatchStore, LocalDispatchStore>();

// --- Autenticación (Semana 4): recupera el estado persistido por el servidor ------------
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

await builder.Build().RunAsync();
