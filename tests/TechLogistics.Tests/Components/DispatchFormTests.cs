using Bunit;
using Microsoft.AspNetCore.Components;
using TechLogistics.Components;
using TechLogistics.Shared.Models;
using Xunit;

namespace TechLogistics.Tests.Components;

public class DispatchFormTests : TestContext
{
    [Fact]
    public void Submit_ConCamposVacios_NoInvocaElCallback_YMuestraErrores()
    {
        DispatchRequest? recibido = null;
        var cut = RenderComponent<DispatchForm>(parameters => parameters
            .Add(p => p.AgenteId, "AGT-0001")
            .Add(p => p.OnDespachoRegistrado, EventCallback.Factory.Create<DispatchRequest>(this, d => recibido = d)));

        cut.Find("form").Submit();

        Assert.Null(recibido);
        Assert.Contains("tl-form-errors", cut.Markup);
    }

    [Fact]
    public void Submit_ConDatosValidos_InvocaElCallbackConElAgenteIdAsignado()
    {
        DispatchRequest? recibido = null;
        var cut = RenderComponent<DispatchForm>(parameters => parameters
            .Add(p => p.AgenteId, "AGT-0007")
            .Add(p => p.OnDespachoRegistrado, EventCallback.Factory.Create<DispatchRequest>(this, d => recibido = d)));

        cut.Find("#sku").Change("TL-00042");
        cut.Find("#centro").Change("Guatemala");
        cut.Find("#cantidad").Change("15");
        cut.Find("form").Submit();

        Assert.NotNull(recibido);
        Assert.Equal("TL-00042", recibido!.Sku);
        Assert.Equal("Guatemala", recibido.CentroOrigen);
        Assert.Equal(15, recibido.Cantidad);
        Assert.Equal("AGT-0007", recibido.AgenteId);
    }

    [Fact]
    public void Render_MuestraLosCentrosDisponiblesProvistosPorParametro()
    {
        var cut = RenderComponent<DispatchForm>(parameters => parameters
            .Add(p => p.AgenteId, "AGT-0001")
            .Add(p => p.CentrosDisponibles, new[] { "Guatemala", "San José" }));

        var opciones = cut.FindAll("#centro option");
        Assert.Equal(3, opciones.Count);
        Assert.Contains(opciones, o => o.TextContent == "Guatemala");
        Assert.Contains(opciones, o => o.TextContent == "San José");
    }
}
