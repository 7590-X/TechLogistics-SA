using TechLogistics.Shared.Models;
using TechLogistics.Web.Services;

namespace TechLogistics.Web.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var catalogo = app.MapGroup("/api/catalogo").WithTags("Catálogo");

        catalogo.MapGet("/", (InMemoryCatalogRepository repo) => Results.Ok(repo.ObtenerTodos()))
            .RequireAuthorization();

        catalogo.MapGet("/{sku}", (string sku, InMemoryCatalogRepository repo) =>
        {
            var item = repo.ObtenerPorSku(sku);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        }).RequireAuthorization();

        catalogo.MapPost("/", (InventoryItem item, InMemoryCatalogRepository repo) =>
        {
            var creado = repo.Crear(item);
            return Results.Created($"/api/catalogo/{creado.Sku}", creado);
        }).RequireAuthorization(policy => policy.RequireRole("GerenteBodega"));

        catalogo.MapPut("/{sku}", (string sku, InventoryItem item, InMemoryCatalogRepository repo) =>
            repo.Actualizar(sku, item) ? Results.NoContent() : Results.NotFound())
            .RequireAuthorization(policy => policy.RequireRole("GerenteBodega"));

        catalogo.MapDelete("/{sku}", (string sku, InMemoryCatalogRepository repo) =>
            repo.Eliminar(sku) ? Results.NoContent() : Results.NotFound())
            .RequireAuthorization(policy => policy.RequireRole("GerenteBodega"));

        var despachos = app.MapGroup("/api/despachos").WithTags("Despachos");

        despachos.MapPost("/sincronizar", (List<DispatchRequest> pendientes, InMemoryCatalogRepository repo) =>
        {
            var procesados = repo.RegistrarDespachosSincronizados(pendientes);
            return Results.Ok(new { Procesados = procesados });
        }).RequireAuthorization(policy => policy.RequireRole("AgenteCampo"));
    }
}
