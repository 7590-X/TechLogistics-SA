using System.ComponentModel;
using TechLogistics.Shared.Models;

namespace TechLogistics.Web.Services;

public interface IAppStateContainer : INotifyPropertyChanged
{
    IReadOnlyList<InventoryItem> InventarioActual { get; }
    Task InicializarAsync();
    void ActualizarInventario(IEnumerable<InventoryItem> items);
}
