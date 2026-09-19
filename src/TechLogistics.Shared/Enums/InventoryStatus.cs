namespace TechLogistics.Shared.Enums;

/// <summary>
/// Estado operativo de un ítem de inventario en un centro de distribución.
/// </summary>
public enum InventoryStatus
{
    Disponible,
    StockBajo,
    Agotado,
    EnTransito,
    Retenido
}
