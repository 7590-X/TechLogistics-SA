using System.ComponentModel;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.JSInterop;
using Moq;
using TechLogistics.Shared.Models;
using TechLogistics.Web.Services;
using Xunit;

namespace TechLogistics.Tests.Services;

public class AppStateContainerTests
{
    private static AppStateContainer CrearSut(Mock<IJSRuntime>? jsRuntimeMock = null)
    {
        jsRuntimeMock ??= new Mock<IJSRuntime>();
        var protectedStorage = new ProtectedLocalStorage(jsRuntimeMock.Object, new EphemeralDataProtectionProvider());
        return new AppStateContainer(protectedStorage);
    }

    [Fact]
    public void ActualizarInventario_AsignaLaListaProvista()
    {
        var sut = CrearSut();
        var items = new List<InventoryItem> { new() { Sku = "TL-00001", Cantidad = 10 } };

        sut.ActualizarInventario(items);

        Assert.Single(sut.InventarioActual);
        Assert.Equal("TL-00001", sut.InventarioActual[0].Sku);
    }

    [Fact]
    public void ActualizarInventario_DisparaPropertyChanged_ParaInventarioActual()
    {
        var sut = CrearSut();
        var propiedadNotificada = string.Empty;
        sut.PropertyChanged += (_, e) => propiedadNotificada = e.PropertyName;

        sut.ActualizarInventario(new List<InventoryItem> { new() { Sku = "TL-00002" } });

        Assert.Equal(nameof(IAppStateContainer.InventarioActual), propiedadNotificada);
    }

    [Fact]
    public void ActualizarInventario_MultiplesSuscriptores_TodosSonNotificados()
    {
        var sut = CrearSut();
        var notificacionesRecibidas = 0;
        PropertyChangedEventHandler handler1 = (_, _) => notificacionesRecibidas++;
        PropertyChangedEventHandler handler2 = (_, _) => notificacionesRecibidas++;
        sut.PropertyChanged += handler1;
        sut.PropertyChanged += handler2;

        sut.ActualizarInventario(new List<InventoryItem>());

        Assert.Equal(2, notificacionesRecibidas);
    }
}
