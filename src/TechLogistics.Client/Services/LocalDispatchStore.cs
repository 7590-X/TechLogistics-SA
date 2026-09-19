using System.Text.Json;
using Microsoft.JSInterop;
using TechLogistics.Shared.Models;

namespace TechLogistics.Client.Services;

/// <summary>
/// Implementación de <see cref="ILocalDispatchStore"/> basada en localStorage del navegador,
/// vía JS interop directo.
/// </summary>
public class LocalDispatchStore : ILocalDispatchStore
{
    private const string StorageKey = "techlogistics.despachos.pendientes";
    private readonly IJSRuntime _jsRuntime;

    public LocalDispatchStore(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task GuardarAsync(DispatchRequest despacho)
    {
        var lista = await ObtenerPendientesAsync();
        lista.Add(despacho);
        await PersistirAsync(lista);
    }

    public async Task<List<DispatchRequest>> ObtenerPendientesAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (string.IsNullOrWhiteSpace(json)) return new List<DispatchRequest>();
        return JsonSerializer.Deserialize<List<DispatchRequest>>(json) ?? new List<DispatchRequest>();
    }

    public async Task MarcarSincronizadoAsync(Guid despachoId)
    {
        var lista = await ObtenerPendientesAsync();
        var item = lista.FirstOrDefault(d => d.Id == despachoId);
        if (item is null) return;
        item.Sincronizado = true;
        await PersistirAsync(lista);
    }

    private async Task PersistirAsync(List<DispatchRequest> lista)
    {
        var json = JsonSerializer.Serialize(lista);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }
}
