using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using TechLogistics.Shared.Models;

namespace TechLogistics.Web.Services;

public class AppStateContainer : IAppStateContainer
{
    private const string StorageKey = "techlogistics.inventario.snapshot";
    private readonly ProtectedLocalStorage _protectedStorage;

    private List<InventoryItem> _inventarioActual = new();

    public AppStateContainer(ProtectedLocalStorage protectedStorage)
    {
        _protectedStorage = protectedStorage;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<InventoryItem> InventarioActual => _inventarioActual;

    public async Task InicializarAsync()
    {
        if (_inventarioActual.Count > 0) return;

        try
        {
            var resultado = await _protectedStorage.GetAsync<string>(StorageKey);
            if (resultado.Success && !string.IsNullOrWhiteSpace(resultado.Value))
            {
                var items = JsonSerializer.Deserialize<List<InventoryItem>>(resultado.Value);
                if (items is not null)
                {
                    _inventarioActual = items;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InventarioActual)));
                }
            }
        }
        catch (InvalidOperationException)
        {
        }
    }

    public void ActualizarInventario(IEnumerable<InventoryItem> items)
    {
        _inventarioActual = items.ToList();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InventarioActual)));
        _ = PersistirAsync();
    }

    private async Task PersistirAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_inventarioActual);
            await _protectedStorage.SetAsync(StorageKey, json);
        }
        catch (InvalidOperationException)
        {
        }
    }
}
