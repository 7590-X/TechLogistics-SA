using System.Net.Http.Json;
using TechLogistics.Shared.Models;

namespace TechLogistics.Shared.Services;

/// <summary>
/// Implementación del cliente REST usando <see cref="HttpClient"/> tipado (System.Net.Http.Json).
/// Se registra vía <c>AddHttpClient&lt;ICatalogApiClient, CatalogApiClient&gt;</c> en el
/// cliente WASM, apuntando a la BaseAddress del propio host que sirve la aplicación.
/// </summary>
public class CatalogApiClient : ICatalogApiClient
{
    private readonly HttpClient _http;

    public CatalogApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<InventoryItem>> ObtenerCatalogoAsync(CancellationToken ct = default)
    {
        var resultado = await _http.GetFromJsonAsync<List<InventoryItem>>("api/catalogo", ct);
        return resultado ?? new List<InventoryItem>();
    }

    public async Task<InventoryItem?> ObtenerPorSkuAsync(string sku, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"api/catalogo/{Uri.EscapeDataString(sku)}", ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<InventoryItem>(cancellationToken: ct);
    }

    public async Task<InventoryItem> CrearAsync(InventoryItem item, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/catalogo", item, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InventoryItem>(cancellationToken: ct) ?? item;
    }

    public async Task<bool> ActualizarAsync(string sku, InventoryItem item, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"api/catalogo/{Uri.EscapeDataString(sku)}", item, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarAsync(string sku, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"api/catalogo/{Uri.EscapeDataString(sku)}", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<int> SincronizarDespachosAsync(IEnumerable<DispatchRequest> pendientes, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/despachos/sincronizar", pendientes, ct);
        response.EnsureSuccessStatusCode();
        var resultado = await response.Content.ReadFromJsonAsync<SincronizacionResultado>(cancellationToken: ct);
        return resultado?.Procesados ?? 0;
    }

    private record SincronizacionResultado(int Procesados);
}
