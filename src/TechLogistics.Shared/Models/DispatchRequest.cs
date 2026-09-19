using System.ComponentModel.DataAnnotations;

namespace TechLogistics.Shared.Models;

/// <summary>
/// Representa una solicitud de salida de inventario registrada por un agente de campo.
/// Se serializa a JSON tanto para persistencia local (offline) como para el envío
/// posterior a la API REST cuando el dispositivo recupera conectividad.
/// </summary>
public class DispatchRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "El SKU es obligatorio.")]
    public string Sku { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe indicar el centro de distribución de origen.")]
    public string CentroOrigen { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }

    [Required(ErrorMessage = "Debe indicar el código del agente que despacha.")]
    public string AgenteId { get; set; } = string.Empty;

    public string? Observaciones { get; set; }

    public DateTimeOffset FechaRegistro { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Indica si el registro ya fue sincronizado con el backend (relevante para
    /// el escenario offline-first del módulo InteractiveWebAssembly).
    /// </summary>
    public bool Sincronizado { get; set; }
}
