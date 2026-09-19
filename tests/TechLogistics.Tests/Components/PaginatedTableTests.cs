using Bunit;
using Microsoft.AspNetCore.Components;
using TechLogistics.Components;
using Xunit;

namespace TechLogistics.Tests.Components;

public class PaginatedTableTests : TestContext
{
    private static RenderFragment HeaderFragment => builder =>
    {
        builder.OpenElement(0, "th");
        builder.AddContent(1, "Valor");
        builder.CloseElement();
    };

    private static RenderFragment<int> RowFragment => item => builder =>
    {
        builder.OpenElement(0, "td");
        builder.AddContent(1, item);
        builder.CloseElement();
    };

    [Fact]
    public void Render_SinItems_MuestraMensajeVacio()
    {
        var cut = RenderComponent<PaginatedTable<int>>(parameters => parameters
            .Add(p => p.Items, Enumerable.Empty<int>())
            .Add(p => p.HeaderTemplate, HeaderFragment)
            .Add(p => p.RowTemplate, RowFragment)
            .Add(p => p.EmptyText, "Sin datos de prueba"));

        Assert.Contains("Sin datos de prueba", cut.Markup);
    }

    [Fact]
    public void Render_ConMasItemsQueElPageSize_MuestraSoloLaPrimeraPagina()
    {
        var items = Enumerable.Range(1, 25);

        var cut = RenderComponent<PaginatedTable<int>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.PageSize, 10)
            .Add(p => p.HeaderTemplate, HeaderFragment)
            .Add(p => p.RowTemplate, RowFragment));

        var filas = cut.FindAll("tbody tr");
        Assert.Equal(10, filas.Count);
        Assert.Contains("Página 1 de 3", cut.Markup);
    }

    [Fact]
    public void ClicSiguiente_AvanzaALaSegundaPaginaYNotificaElEvento()
    {
        var items = Enumerable.Range(1, 25);
        var paginaNotificada = 0;

        var cut = RenderComponent<PaginatedTable<int>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.PageSize, 10)
            .Add(p => p.HeaderTemplate, HeaderFragment)
            .Add(p => p.RowTemplate, RowFragment)
            .Add(p => p.OnPageChanged, EventCallback.Factory.Create<int>(this, pagina => paginaNotificada = pagina)));

        var botones = cut.FindAll("button.tl-btn");
        botones[1].Click();

        Assert.Contains("Página 2 de 3", cut.Markup);
        Assert.Equal(2, paginaNotificada);
    }

    [Fact]
    public void BotonAnterior_EnLaPrimeraPagina_EstaDeshabilitado()
    {
        var items = Enumerable.Range(1, 25);

        var cut = RenderComponent<PaginatedTable<int>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.PageSize, 10)
            .Add(p => p.HeaderTemplate, HeaderFragment)
            .Add(p => p.RowTemplate, RowFragment));

        var botonAnterior = cut.FindAll("button.tl-btn")[0];
        Assert.True(botonAnterior.HasAttribute("disabled"));
    }
}
