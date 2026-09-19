using TechLogistics.Shared.Models;

namespace TechLogistics.Web.Services;

public interface ITelemetryAggregator
{
    event Action<InventoryItem>? InventarioActualizado;
    void PublicarActualizacion(InventoryItem item);
}
