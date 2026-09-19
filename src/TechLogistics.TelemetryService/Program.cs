using TechLogistics.TelemetryService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<TelemetryBroadcastChannel>();
builder.Services.AddHostedService<SimulatedTelemetryProducer>();

var app = builder.Build();

app.MapGrpcService<InventoryTelemetryService>();
app.MapGet("/", () => "TechLogistics.TelemetryService activo. Use un cliente gRPC (HTTP/2) para consumir InventoryTelemetry.");

app.Run();
