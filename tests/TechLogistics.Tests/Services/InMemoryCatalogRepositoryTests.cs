using TechLogistics.Shared.Models;
using TechLogistics.Web.Services;
using Xunit;

namespace TechLogistics.Tests.Services;

public class InMemoryCatalogRepositoryTests
{
    [Fact]
    public void Crear_AgregaUnNuevoItemConsultablePorSku()
    {
        var repo = new InMemoryCatalogRepository();
        var item = new InventoryItem { Sku = "TL-99999", Nombre = "Ítem de prueba", Cantidad = 5 };

        repo.Crear(item);
        var recuperado = repo.ObtenerPorSku("TL-99999");

        Assert.NotNull(recuperado);
        Assert.Equal("Ítem de prueba", recuperado!.Nombre);
    }

    [Fact]
    public void Actualizar_ConSkuInexistente_DevuelveFalse()
    {
        var repo = new InMemoryCatalogRepository();
        var resultado = repo.Actualizar("TL-NO-EXISTE", new InventoryItem());
        Assert.False(resultado);
    }

    [Fact]
    public void Eliminar_ConSkuExistente_LoRemueveDelCatalogo()
    {
        var repo = new InMemoryCatalogRepository();
        repo.Crear(new InventoryItem { Sku = "TL-88888" });

        var eliminado = repo.Eliminar("TL-88888");

        Assert.True(eliminado);
        Assert.Null(repo.ObtenerPorSku("TL-88888"));
    }

    [Fact]
    public void RegistrarDespachosSincronizados_DescuentaStockDelItemCorrespondiente()
    {
        var repo = new InMemoryCatalogRepository();
        repo.Crear(new InventoryItem { Sku = "TL-77777", Cantidad = 50 });
        var despacho = new DispatchRequest { Sku = "TL-77777", Cantidad = 20 };

        repo.RegistrarDespachosSincronizados(new[] { despacho });

        var item = repo.ObtenerPorSku("TL-77777");
        Assert.Equal(30, item!.Cantidad);
        Assert.True(despacho.Sincronizado);
    }

    [Fact]
    public void RegistrarDespachosSincronizados_NuncaDejaCantidadNegativa()
    {
        var repo = new InMemoryCatalogRepository();
        repo.Crear(new InventoryItem { Sku = "TL-66666", Cantidad = 5 });
        var despacho = new DispatchRequest { Sku = "TL-66666", Cantidad = 999 };

        repo.RegistrarDespachosSincronizados(new[] { despacho });

        var item = repo.ObtenerPorSku("TL-66666");
        Assert.Equal(0, item!.Cantidad);
    }
}
