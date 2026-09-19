using TechLogistics.Shared.Enums;
using TechLogistics.Shared.Models;
using Xunit;

namespace TechLogistics.Tests.Models;

public class InventoryItemTests
{
    [Fact]
    public void CalcularStatus_CuandoCantidadEsCero_DevuelveAgotado()
    {
        var item = new InventoryItem { Cantidad = 0, CantidadMinima = 10 };
        var resultado = item.CalcularStatus();
        Assert.Equal(InventoryStatus.Agotado, resultado);
    }

    [Fact]
    public void CalcularStatus_CuandoCantidadEsNegativa_DevuelveAgotado()
    {
        var item = new InventoryItem { Cantidad = -5, CantidadMinima = 10 };
        var resultado = item.CalcularStatus();
        Assert.Equal(InventoryStatus.Agotado, resultado);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(10, 10)]
    public void CalcularStatus_CuandoCantidadEsMenorOIgualAlMinimo_DevuelveStockBajo(int cantidad, int minima)
    {
        var item = new InventoryItem { Cantidad = cantidad, CantidadMinima = minima };
        var resultado = item.CalcularStatus();
        Assert.Equal(InventoryStatus.StockBajo, resultado);
    }

    [Fact]
    public void CalcularStatus_CuandoCantidadSuperaElMinimo_DevuelveDisponible()
    {
        var item = new InventoryItem { Cantidad = 50, CantidadMinima = 10 };
        var resultado = item.CalcularStatus();
        Assert.Equal(InventoryStatus.Disponible, resultado);
    }
}
