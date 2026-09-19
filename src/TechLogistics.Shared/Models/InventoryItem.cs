using TechLogistics.Shared.Enums;

namespace TechLogistics.Shared.Models;

/// <summary>
/// Representa un ítem de inventario dentro de un centro de distribución.
/// Modelo compartido entre el servidor (InteractiveServer) y el cliente (InteractiveWebAssembly).
/// </summary>
public class InventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Sku { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string CentroDistribucion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int CantidadMinima { get; set; }
    public InventoryStatus Status { get; set; } = InventoryStatus.Disponible;
    public DateTimeOffset UltimaActualizacion { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Calcula el estado derivado a partir de la cantidad actual vs. la mínima.
    /// Centraliza la regla de negocio para evitar duplicarla en cada componente.
    /// </summary>
    public InventoryStatus CalcularStatus()
    {
        if (Cantidad <= 0) return InventoryStatus.Agotado;
        if (Cantidad <= CantidadMinima) return InventoryStatus.StockBajo;
        return InventoryStatus.Disponible;
    }
}
