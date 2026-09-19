using System.Collections.Concurrent;
using System.Threading.Channels;
using TechLogistics.Grpc;

namespace TechLogistics.TelemetryService.Services;

public class TelemetryBroadcastChannel
{
    private readonly ConcurrentDictionary<ChannelReader<InventoryUpdate>, ChannelWriter<InventoryUpdate>> _suscriptores = new();

    public ChannelReader<InventoryUpdate> Subscribe()
    {
        var channel = Channel.CreateUnbounded<InventoryUpdate>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        _suscriptores[channel.Reader] = channel.Writer;
        return channel.Reader;
    }

    public void Unsubscribe(ChannelReader<InventoryUpdate> reader)
    {
        if (_suscriptores.TryRemove(reader, out var writer))
        {
            writer.TryComplete();
        }
    }

    public void Publish(InventoryUpdate update)
    {
        foreach (var writer in _suscriptores.Values)
        {
            writer.TryWrite(update);
        }
    }
}
