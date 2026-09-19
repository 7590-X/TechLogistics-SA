using TechLogistics.Shared.Models;

namespace TechLogistics.Client.Services;

/// <summary>
/// Abstracción de persistencia local para despachos registrados sin conexión.
/// </summary>
public interface ILocalDispatchStore
{
    Task GuardarAsync(DispatchRequest despacho);
    Task<List<DispatchRequest>> ObtenerPendientesAsync();
    Task MarcarSincronizadoAsync(Guid despachoId);
}
