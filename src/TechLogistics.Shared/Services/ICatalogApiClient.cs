using TechLogistics.Shared.Models;

namespace TechLogistics.Shared.Services;

/// <summary>
/// Contrato del cliente REST para el CRUD de catálogo e historial de inventario.
/// Se define en el proyecto Shared para que la MISMA interfaz e implementación
/// puedan inyectarse tanto en el host InteractiveServer como en el cliente
/// InteractiveWebAssembly (Principio de Inversión de Dependencias).
/// </summary>
public interface ICatalogApiClient
{
    Task<List<InventoryItem>> ObtenerCatalogoAsync(CancellationToken ct = default);
    Task<InventoryItem?> ObtenerPorSkuAsync(string sku, CancellationToken ct = default);
    Task<InventoryItem> CrearAsync(InventoryItem item, CancellationToken ct = default);
    Task<bool> ActualizarAsync(string sku, InventoryItem item, CancellationToken ct = default);
    Task<bool> EliminarAsync(string sku, CancellationToken ct = default);

    /// <summary>Envía un lote de despachos pendientes (registrados offline) para su sincronización.</summary>
    Task<int> SincronizarDespachosAsync(IEnumerable<DispatchRequest> pendientes, CancellationToken ct = default);
}
