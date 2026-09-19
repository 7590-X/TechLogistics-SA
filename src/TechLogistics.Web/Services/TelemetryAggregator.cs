using TechLogistics.Shared.Models;

namespace TechLogistics.Web.Services;

public class TelemetryAggregator : ITelemetryAggregator
{
    public event Action<InventoryItem>? InventarioActualizado;

    public void PublicarActualizacion(InventoryItem item)
    {
        InventarioActualizado?.Invoke(item);
    }
}
