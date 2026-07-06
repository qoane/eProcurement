using System.Text.Json;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.Api.Controllers;

public sealed record PageDatasourceDto(string Entity, string Mode = "Metadata", string? Endpoint = null, string? KeyField = null);
public sealed record PageLayoutDto(string Template, int Columns = 12, string Density = "Comfortable", List<string>? Regions = null);
public sealed record PageToolbarItemDto(string Code, string Label, string Kind = "Button", string? Icon = null, string? ActionCode = null);
public sealed record PageActionDto(string Code, string Label, string Kind = "Command", string? Target = null, string? Confirmation = null);
public sealed record PageFilterDto(string Code, string Label, string Field, string Operator = "Equals", string? DefaultValue = null);
public sealed record PageColumnDto(string Code, string Label, string Field, int DisplayOrder = 0, bool Sortable = false, bool Searchable = false);
public sealed record PageComponentDto(string Code, string Name, string ComponentType, string Region = "main", int DisplayOrder = 0, JsonElement? Configuration = null);
public sealed record PagePermissionDto(string Role, string Access = "View");
public sealed record PageNavigationDto(string Route, string? ParentRoute = null, string? MenuGroup = null, bool ShowInNavigation = true);
public sealed record PageDesignerDto(Guid? Id, string Code, string Name, string Description, PageType PageType, PageDatasourceDto Datasource, PageLayoutDto Layout, List<PageToolbarItemDto> Toolbar, List<PageActionDto> Actions, List<PageFilterDto> Filters, List<PageColumnDto> Columns, List<PageComponentDto> Components, List<PagePermissionDto> Permissions, PageNavigationDto Navigation, MetadataStatus Status = MetadataStatus.Draft, int Version = 1);

[ApiController, Route("api/page-definitions")]
public sealed class PageDefinitionsController(EProcurementDbContext db) : ControllerBase
{
    static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [HttpGet]
    public async Task<ActionResult<List<PageDesignerDto>>> List(CancellationToken ct) => Ok((await db.PageDefinitions.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct)).Select(ToDto).ToList());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PageDesignerDto>> Get(Guid id, CancellationToken ct) => await db.PageDefinitions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) is { } item ? Ok(ToDto(item)) : NotFound();

    [HttpPost]
    public async Task<ActionResult<PageDesignerDto>> Create(PageDesignerDto dto, CancellationToken ct)
    {
        var page = new PageDefinition(dto.Code, dto.Name, dto.Description, dto.PageType, Serialize(dto.Datasource), Serialize(dto.Layout), Serialize(dto.Toolbar), Serialize(dto.Actions), Serialize(dto.Filters), Serialize(dto.Columns), Serialize(dto.Components), Serialize(dto.Permissions), Serialize(dto.Navigation), dto.Version, dto.Status);
        db.PageDefinitions.Add(page);
        await db.SaveChangesAsync(ct);
        return Created($"/api/page-definitions/{page.Id}", ToDto(page));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PageDesignerDto>> Update(Guid id, PageDesignerDto dto, CancellationToken ct)
    {
        var page = await db.PageDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (page is null) return NotFound();
        db.Entry(page).CurrentValues[nameof(PageDefinition.Code)] = dto.Code;
        db.Entry(page).CurrentValues[nameof(PageDefinition.Name)] = dto.Name;
        db.Entry(page).CurrentValues[nameof(PageDefinition.Description)] = dto.Description;
        db.Entry(page).CurrentValues[nameof(PageDefinition.PageType)] = dto.PageType;
        db.Entry(page).CurrentValues[nameof(PageDefinition.DatasourceJson)] = Serialize(dto.Datasource);
        db.Entry(page).CurrentValues[nameof(PageDefinition.LayoutJson)] = Serialize(dto.Layout);
        db.Entry(page).CurrentValues[nameof(PageDefinition.ToolbarJson)] = Serialize(dto.Toolbar);
        db.Entry(page).CurrentValues[nameof(PageDefinition.ActionsJson)] = Serialize(dto.Actions);
        db.Entry(page).CurrentValues[nameof(PageDefinition.FiltersJson)] = Serialize(dto.Filters);
        db.Entry(page).CurrentValues[nameof(PageDefinition.ColumnsJson)] = Serialize(dto.Columns);
        db.Entry(page).CurrentValues[nameof(PageDefinition.ComponentsJson)] = Serialize(dto.Components);
        db.Entry(page).CurrentValues[nameof(PageDefinition.PermissionsJson)] = Serialize(dto.Permissions);
        db.Entry(page).CurrentValues[nameof(PageDefinition.NavigationJson)] = Serialize(dto.Navigation);
        db.Entry(page).CurrentValues[nameof(PageDefinition.Status)] = dto.Status;
        db.Entry(page).CurrentValues[nameof(PageDefinition.Version)] = dto.Version;
        db.Entry(page).CurrentValues[nameof(PageDefinition.Modified)] = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return Ok(ToDto(await db.PageDefinitions.AsNoTracking().SingleAsync(x => x.Id == id, ct)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var page = await db.PageDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (page is null) return NotFound();
        db.PageDefinitions.Remove(page);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    static PageDesignerDto ToDto(PageDefinition p) => new(p.Id, p.Code, p.Name, p.Description, p.PageType, Deserialize<PageDatasourceDto>(p.DatasourceJson, new PageDatasourceDto("Supplier")), Deserialize<PageLayoutDto>(p.LayoutJson, new PageLayoutDto("DataGrid")), DeserializeList<PageToolbarItemDto>(p.ToolbarJson), DeserializeList<PageActionDto>(p.ActionsJson), DeserializeList<PageFilterDto>(p.FiltersJson), DeserializeList<PageColumnDto>(p.ColumnsJson), DeserializeList<PageComponentDto>(p.ComponentsJson), DeserializeList<PagePermissionDto>(p.PermissionsJson), Deserialize<PageNavigationDto>(p.NavigationJson, new PageNavigationDto("/app")), p.Status, p.Version);
    static string Serialize<T>(T item) => JsonSerializer.Serialize(item, JsonOptions);
    static T Deserialize<T>(string json, T fallback) => string.IsNullOrWhiteSpace(json) ? fallback : JsonSerializer.Deserialize<T>(json, JsonOptions) ?? fallback;
    static List<T> DeserializeList<T>(string json) => string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? [];
}
