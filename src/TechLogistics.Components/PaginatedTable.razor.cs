using Microsoft.AspNetCore.Components;

namespace TechLogistics.Components;

/// <summary>
/// Tabla paginada genérica y reutilizable. No conoce el tipo concreto de dato (TItem),
/// por lo que puede usarse tanto para inventario como para historial de despachos,
/// respetando el Principio de Responsabilidad Única y Abierto/Cerrado (SOLID).
/// </summary>
public partial class PaginatedTable<TItem>
{
    /// <summary>Origen de datos completo; el componente se encarga solo de paginar y renderizar.</summary>
    [Parameter, EditorRequired]
    public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();

    /// <summary>Plantilla de encabezado (columnas &lt;th&gt;).</summary>
    [Parameter, EditorRequired]
    public RenderFragment HeaderTemplate { get; set; } = default!;

    /// <summary>Plantilla de fila; recibe el ítem actual y produce las celdas &lt;td&gt;.</summary>
    [Parameter, EditorRequired]
    public RenderFragment<TItem> RowTemplate { get; set; } = default!;

    /// <summary>Cantidad de registros por página.</summary>
    [Parameter]
    public int PageSize { get; set; } = 10;

    /// <summary>Texto mostrado cuando no hay registros.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "No hay registros para mostrar.";

    /// <summary>Evento notificado al padre cuando cambia de página (permite lazy-loading remoto si se requiere).</summary>
    [Parameter]
    public EventCallback<int> OnPageChanged { get; set; }

    private int CurrentPage { get; set; } = 1;

    private int TotalPages => Items is null || !Items.Any()
        ? 0
        : (int)Math.Ceiling(Items.Count() / (double)PageSize);

    private List<TItem> PagedItems => Items?
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize)
        .ToList() ?? new List<TItem>();

    protected override void OnParametersSet()
    {
        if (CurrentPage > TotalPages && TotalPages > 0)
        {
            CurrentPage = TotalPages;
        }
    }

    private async Task GoToPage(int page)
    {
        if (page < 1 || page > TotalPages || page == CurrentPage) return;
        CurrentPage = page;
        await OnPageChanged.InvokeAsync(CurrentPage);
    }
}
