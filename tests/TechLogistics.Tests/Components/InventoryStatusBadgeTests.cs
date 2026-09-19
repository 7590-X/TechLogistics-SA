using Bunit;
using TechLogistics.Components;
using TechLogistics.Shared.Enums;
using Xunit;

namespace TechLogistics.Tests.Components;

public class InventoryStatusBadgeTests : TestContext
{
    [Theory]
    [InlineData(InventoryStatus.Disponible, "Disponible", "tl-badge-success")]
    [InlineData(InventoryStatus.StockBajo, "Stock bajo", "tl-badge-warning")]
    [InlineData(InventoryStatus.Agotado, "Agotado", "tl-badge-danger")]
    [InlineData(InventoryStatus.EnTransito, "En tránsito", "tl-badge-info")]
    [InlineData(InventoryStatus.Retenido, "Retenido", "tl-badge-neutral")]
    public void Render_ParaCadaEstado_MuestraTextoYClaseCorrectos(InventoryStatus status, string textoEsperado, string claseEsperada)
    {
        var cut = RenderComponent<InventoryStatusBadge>(parameters => parameters
            .Add(p => p.Status, status));

        Assert.Contains(textoEsperado, cut.Markup);
        Assert.Contains(claseEsperada, cut.Find("span.tl-badge").ClassList);
    }

    [Fact]
    public void Render_ConTextoPersonalizado_SobreescribeLaEtiquetaPorDefecto()
    {
        var cut = RenderComponent<InventoryStatusBadge>(parameters => parameters
            .Add(p => p.Status, InventoryStatus.Disponible)
            .Add(p => p.TextoPersonalizado, "Listo para despacho"));

        Assert.Contains("Listo para despacho", cut.Markup);
        Assert.DoesNotContain(">Disponible<", cut.Markup);
    }
}
