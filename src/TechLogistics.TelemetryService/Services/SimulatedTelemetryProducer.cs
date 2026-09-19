using TechLogistics.Grpc;

namespace TechLogistics.TelemetryService.Services;

public class SimulatedTelemetryProducer : BackgroundService
{
    private static readonly string[] Centros = { "Guatemala", "San Salvador", "Tegucigalpa", "San José" };
    private static readonly string[] Skus = Enumerable.Range(1, 24).Select(i => $"TL-{i:D5}").ToArray();

    private readonly TelemetryBroadcastChannel _channel;

    public SimulatedTelemetryProducer(TelemetryBroadcastChannel channel)
    {
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            var sku = Skus[random.Next(Skus.Length)];
            var update = new InventoryUpdate
            {
                Sku = sku,
                Nombre = $"Ítem de catálogo {sku}",
                CentroDistribucion = Centros[random.Next(Centros.Length)],
                Cantidad = random.Next(0, 200),
                CantidadMinima = 20,
                TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            _channel.Publish(update);
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
