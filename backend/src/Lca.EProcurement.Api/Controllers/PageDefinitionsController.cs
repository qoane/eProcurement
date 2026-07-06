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
public sealed record PageDesignerDto(Guid? Id, string Code, string Name, string Description, Guid? ApplicationId, PageType PageType, string Route, string Icon, PageDatasourceDto Datasource, Guid? LayoutId, PageLayoutDto Layout, List<PageToolbarItemDto> Toolbar, List<PageActionDto> Actions, List<PageFilterDto> Filters, List<PageColumnDto> Columns, List<PageComponentDto> Components, List<PagePermissionDto> Permissions, PageNavigationDto Navigation, MetadataStatus Status = MetadataStatus.Draft, int Version = 1, Guid? PublishedVersionId = null);

public interface IPageDefinitionApplicationService
{
    Task<List<PageDesignerDto>> ListAsync(CancellationToken ct = default);
    Task<PageDesignerDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<PageDesignerDto> CreateAsync(PageDesignerDto dto, CancellationToken ct = default);
    Task<PageDesignerDto?> UpdateAsync(Guid id, PageDesignerDto dto, CancellationToken ct = default);
    Task<PageDesignerDto?> PublishAsync(Guid id, CancellationToken ct = default);
    Task<PageDesignerDto?> ArchiveAsync(Guid id, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class PageDefinitionApplicationService(EProcurementDbContext db) : IPageDefinitionApplicationService
{
    static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public async Task<List<PageDesignerDto>> ListAsync(CancellationToken ct = default) => (await db.PageDefinitions.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct)).Select(ToDto).ToList();
    public async Task<PageDesignerDto?> GetAsync(Guid id, CancellationToken ct = default) => await db.PageDefinitions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) is { } p ? ToDto(p) : null;
    public async Task<PageDesignerDto> CreateAsync(PageDesignerDto dto, CancellationToken ct = default)
    {
        var page = new PageDefinition(dto.Code, dto.Name, dto.Description, dto.ApplicationId, dto.PageType, dto.Route, dto.Icon, Serialize(dto.Datasource), dto.LayoutId, Serialize(dto.Layout), Serialize(dto.Toolbar), Serialize(dto.Actions), Serialize(dto.Filters), Serialize(dto.Columns), Serialize(dto.Components), Serialize(dto.Permissions), Serialize(dto.Navigation), dto.PublishedVersionId, dto.Version, dto.Status, CreatedBy: "admin");
        db.PageDefinitions.Add(page); await db.SaveChangesAsync(ct); return ToDto(page);
    }
    public async Task<PageDesignerDto?> UpdateAsync(Guid id, PageDesignerDto dto, CancellationToken ct = default)
    {
        var p = await db.PageDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct); if (p is null) return null;
        db.Entry(p).CurrentValues[nameof(PageDefinition.Code)] = dto.Code; db.Entry(p).CurrentValues[nameof(PageDefinition.Name)] = dto.Name; db.Entry(p).CurrentValues[nameof(PageDefinition.Description)] = dto.Description; db.Entry(p).CurrentValues[nameof(PageDefinition.ApplicationId)] = dto.ApplicationId; db.Entry(p).CurrentValues[nameof(PageDefinition.PageType)] = dto.PageType; db.Entry(p).CurrentValues[nameof(PageDefinition.Route)] = dto.Route; db.Entry(p).CurrentValues[nameof(PageDefinition.Icon)] = dto.Icon; db.Entry(p).CurrentValues[nameof(PageDefinition.DatasourceJson)] = Serialize(dto.Datasource); db.Entry(p).CurrentValues[nameof(PageDefinition.LayoutId)] = dto.LayoutId; db.Entry(p).CurrentValues[nameof(PageDefinition.LayoutJson)] = Serialize(dto.Layout); db.Entry(p).CurrentValues[nameof(PageDefinition.ToolbarJson)] = Serialize(dto.Toolbar); db.Entry(p).CurrentValues[nameof(PageDefinition.ActionsJson)] = Serialize(dto.Actions); db.Entry(p).CurrentValues[nameof(PageDefinition.FiltersJson)] = Serialize(dto.Filters); db.Entry(p).CurrentValues[nameof(PageDefinition.ColumnsJson)] = Serialize(dto.Columns); db.Entry(p).CurrentValues[nameof(PageDefinition.ComponentsJson)] = Serialize(dto.Components); db.Entry(p).CurrentValues[nameof(PageDefinition.PermissionsJson)] = Serialize(dto.Permissions); db.Entry(p).CurrentValues[nameof(PageDefinition.NavigationJson)] = Serialize(dto.Navigation); db.Entry(p).CurrentValues[nameof(PageDefinition.Status)] = dto.Status; db.Entry(p).CurrentValues[nameof(PageDefinition.Version)] = dto.Version; db.Entry(p).CurrentValues[nameof(PageDefinition.Modified)] = DateTimeOffset.UtcNow; db.Entry(p).CurrentValues[nameof(PageDefinition.ModifiedBy)] = "admin";
        await db.SaveChangesAsync(ct); return await GetAsync(id, ct);
    }
    public async Task<PageDesignerDto?> PublishAsync(Guid id, CancellationToken ct = default)
    {
        var p = await db.PageDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct); if (p is null) return null;
        var newVersionId = Guid.NewGuid(); db.Entry(p).CurrentValues[nameof(PageDefinition.Status)] = MetadataStatus.Active; db.Entry(p).CurrentValues[nameof(PageDefinition.PublishedVersionId)] = newVersionId; db.Entry(p).CurrentValues[nameof(PageDefinition.Version)] = p.Version + 1; db.Entry(p).CurrentValues[nameof(PageDefinition.Modified)] = DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return await GetAsync(id, ct);
    }
    public async Task<PageDesignerDto?> ArchiveAsync(Guid id, CancellationToken ct = default)
    { var p = await db.PageDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct); if (p is null) return null; db.Entry(p).CurrentValues[nameof(PageDefinition.Status)] = MetadataStatus.Archived; db.Entry(p).CurrentValues[nameof(PageDefinition.Modified)] = DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) { var p = await db.PageDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct); if (p is null) return false; db.PageDefinitions.Remove(p); await db.SaveChangesAsync(ct); return true; }
    static PageDesignerDto ToDto(PageDefinition p) => new(p.Id, p.Code, p.Name, p.Description, p.ApplicationId, p.PageType, p.Route, p.Icon, Deserialize(p.DatasourceJson, new PageDatasourceDto("Supplier")), p.LayoutId, Deserialize(p.LayoutJson, new PageLayoutDto("Single Column", Regions: ["main"])), DeserializeList<PageToolbarItemDto>(p.ToolbarJson), DeserializeList<PageActionDto>(p.ActionsJson), DeserializeList<PageFilterDto>(p.FiltersJson), DeserializeList<PageColumnDto>(p.ColumnsJson), DeserializeList<PageComponentDto>(p.ComponentsJson), DeserializeList<PagePermissionDto>(p.PermissionsJson), Deserialize(p.NavigationJson, new PageNavigationDto(p.Route)), p.Status, p.Version, p.PublishedVersionId);
    static string Serialize<T>(T item) => JsonSerializer.Serialize(item, JsonOptions);
    static T Deserialize<T>(string json, T fallback) => string.IsNullOrWhiteSpace(json) ? fallback : JsonSerializer.Deserialize<T>(json, JsonOptions) ?? fallback;
    static List<T> DeserializeList<T>(string json) => string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? [];
}

[ApiController, Route("api/page-definitions")]
public sealed class PageDefinitionsController(IPageDefinitionApplicationService pages) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<List<PageDesignerDto>>> List(CancellationToken ct) => Ok(await pages.ListAsync(ct));
    [HttpGet("{id:guid}")] public async Task<ActionResult<PageDesignerDto>> Get(Guid id, CancellationToken ct) => await pages.GetAsync(id, ct) is { } item ? Ok(item) : NotFound();
    [HttpPost] public async Task<ActionResult<PageDesignerDto>> Create(PageDesignerDto dto, CancellationToken ct) { var page = await pages.CreateAsync(dto, ct); return Created($"/api/page-definitions/{page.Id}", page); }
    [HttpPut("{id:guid}")] public async Task<ActionResult<PageDesignerDto>> Update(Guid id, PageDesignerDto dto, CancellationToken ct) => await pages.UpdateAsync(id, dto, ct) is { } page ? Ok(page) : NotFound();
    [HttpPost("{id:guid}/publish")] public async Task<ActionResult<PageDesignerDto>> Publish(Guid id, CancellationToken ct) => await pages.PublishAsync(id, ct) is { } page ? Ok(page) : NotFound();
    [HttpPost("{id:guid}/archive")] public async Task<ActionResult<PageDesignerDto>> Archive(Guid id, CancellationToken ct) => await pages.ArchiveAsync(id, ct) is { } page ? Ok(page) : NotFound();
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken ct) => await pages.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
