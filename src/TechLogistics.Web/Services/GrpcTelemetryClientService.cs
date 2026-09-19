using Grpc.Net.Client;
using TechLogistics.Grpc;
using TechLogistics.Shared.Models;
using Grpc.Core;

namespace TechLogistics.Web.Services;

public class GrpcTelemetryClientService : BackgroundService
{
    private readonly ITelemetryAggregator _aggregator;
    private readonly ILogger<GrpcTelemetryClientService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public GrpcTelemetryClientService(
        ITelemetryAggregator aggregator,
        ILogger<GrpcTelemetryClientService> logger,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _aggregator = aggregator;
        _logger = logger;
        _configuration = configuration;
        _environment = environment;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var endpoint = _configuration["Grpc:TelemetryEndpoint"] ?? "https://localhost:7100";

        var httpHandler = new HttpClientHandler();
        if (_environment.IsDevelopment())
        {
            // En desarrollo local (especialmente en Linux), permitir el certificado local de ASP.NET Core
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        var channelOptions = new GrpcChannelOptions
        {
            HttpHandler = httpHandler
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(endpoint, channelOptions);
                var client = new InventoryTelemetry.InventoryTelemetryClient(channel);

                using var call = client.StreamInventoryUpdates(new StreamRequest(), cancellationToken: stoppingToken);

                _logger.LogInformation("Conectado al canal gRPC de telemetría en {Endpoint}", endpoint);

                await foreach (var update in call.ResponseStream.ReadAllAsync(stoppingToken))
                {
                    var item = new InventoryItem
                    {
                        Sku = update.Sku,
                        Nombre = update.Nombre,
                        CentroDistribucion = update.CentroDistribucion,
                        Cantidad = update.Cantidad,
                        CantidadMinima = update.CantidadMinima,
                        UltimaActualizacion = DateTimeOffset.FromUnixTimeMilliseconds(update.TimestampUnixMs)
                    };
                    item.Status = item.CalcularStatus();

                    _aggregator.PublicarActualizacion(item);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Conexión gRPC de telemetría interrumpida. Reintentando en 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
