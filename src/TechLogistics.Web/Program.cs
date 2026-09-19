using Microsoft.AspNetCore.Authentication.Cookies;
using TechLogistics.Web.Auth;
using TechLogistics.Web.Components;
using TechLogistics.Web.Endpoints;
using TechLogistics.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de servicios según ciclo de vida (pilar 2 del encargo) --------------------
builder.Services.AddSingleton<ITelemetryAggregator, TelemetryAggregator>();
builder.Services.AddSingleton<InMemoryCatalogRepository>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<DemoUserStore>();

builder.Services.AddScoped<IAppStateContainer, AppStateContainer>();

// --- Render Modes: se habilitan AMBOS modelos de hosting interactivo --------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// --- Autenticación y autorización (Semana 4) --------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, PersistingAuthStateProvider>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Cookie.Name = "TechLogisticsAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("GerenteBodega", p => p.RequireRole("GerenteBodega"));
    options.AddPolicy("AgenteCampo", p => p.RequireRole("AgenteCampo"));
});

// --- Telemetría gRPC (Semana 3) ----------------------------------------------------------
builder.Services.AddHostedService<GrpcTelemetryClientService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TechLogistics.Client._Imports).Assembly);

app.MapCatalogEndpoints();
app.MapAuthEndpoints();

app.Run();
