using System.Collections.Concurrent;
using TechLogistics.Shared.Models;

namespace TechLogistics.Web.Services;

public class InMemoryCatalogRepository
{
    private readonly ConcurrentDictionary<string, InventoryItem> _items = new();
    private readonly ConcurrentBag<DispatchRequest> _despachosSincronizados = new();

    public InMemoryCatalogRepository()
    {
        var centros = new[] { "Guatemala", "San Salvador", "Tegucigalpa", "San José" };
        for (var i = 1; i <= 24; i++)
        {
            var sku = $"TL-{i:D5}";
            _items[sku] = new InventoryItem
            {
                Sku = sku,
                Nombre = $"Ítem de catálogo {i}",
                CentroDistribucion = centros[i % centros.Length],
                Cantidad = Random.Shared.Next(0, 200),
                CantidadMinima = 20,
                UltimaActualizacion = DateTimeOffset.UtcNow
            };
        }
    }

    public IReadOnlyList<InventoryItem> ObtenerTodos() => _items.Values.OrderBy(i => i.Sku).ToList();

    public InventoryItem? ObtenerPorSku(string sku) => _items.GetValueOrDefault(sku);

    public InventoryItem Crear(InventoryItem item)
    {
        item.UltimaActualizacion = DateTimeOffset.UtcNow;
        _items[item.Sku] = item;
        return item;
    }

    public bool Actualizar(string sku, InventoryItem item)
    {
        if (!_items.ContainsKey(sku)) return false;
        item.Sku = sku;
        item.UltimaActualizacion = DateTimeOffset.UtcNow;
        _items[sku] = item;
        return true;
    }

    public bool Eliminar(string sku) => _items.TryRemove(sku, out _);

    public int RegistrarDespachosSincronizados(IEnumerable<DispatchRequest> despachos)
    {
        var lista = despachos.ToList();
        foreach (var despacho in lista)
        {
            despacho.Sincronizado = true;
            _despachosSincronizados.Add(despacho);

            if (_items.TryGetValue(despacho.Sku, out var item))
            {
                item.Cantidad = Math.Max(0, item.Cantidad - despacho.Cantidad);
                item.UltimaActualizacion = DateTimeOffset.UtcNow;
            }
        }
        return lista.Count;
    }
}
