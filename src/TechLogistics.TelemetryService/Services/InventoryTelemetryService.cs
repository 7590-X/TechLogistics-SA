using Grpc.Core;
using TechLogistics.Grpc;

namespace TechLogistics.TelemetryService.Services;

public class InventoryTelemetryService : InventoryTelemetry.InventoryTelemetryBase
{
    private readonly TelemetryBroadcastChannel _broadcastChannel;
    private readonly ILogger<InventoryTelemetryService> _logger;

    public InventoryTelemetryService(TelemetryBroadcastChannel broadcastChannel, ILogger<InventoryTelemetryService> logger)
    {
        _broadcastChannel = broadcastChannel;
        _logger = logger;
    }

    public override async Task StreamInventoryUpdates(
        StreamRequest request,
        IServerStreamWriter<InventoryUpdate> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("Cliente suscrito al stream de telemetría (centro filtro: '{Centro}')", request.CentroDistribucion);

        var reader = _broadcastChannel.Subscribe();
        try
        {
            await foreach (var update in reader.ReadAllAsync(context.CancellationToken))
            {
                if (!string.IsNullOrEmpty(request.CentroDistribucion) &&
                    update.CentroDistribucion != request.CentroDistribucion)
                {
                    continue;
                }

                await responseStream.WriteAsync(update);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _broadcastChannel.Unsubscribe(reader);
        }
    }

    public override Task<PublishAck> PublishUpdate(InventoryUpdate request, ServerCallContext context)
    {
        _broadcastChannel.Publish(request);
        return Task.FromResult(new PublishAck { Aceptado = true, Mensaje = "Actualización aceptada." });
    }
}
