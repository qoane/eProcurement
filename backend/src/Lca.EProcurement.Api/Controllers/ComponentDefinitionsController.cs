using System.Text.Json;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.Api.Controllers;

public sealed record ComponentPropertyDto(string Code, string Name, string DataType, bool Required = false, string? DefaultValue = null, string? HelpText = null, JsonElement? Options = null);
public sealed record ComponentEventDto(string Code, string Name, string Description, JsonElement? PayloadSchema = null);
public sealed record ComponentValidationDto(string Code, string Name, string Expression, string Message);
public sealed record ComponentDesignMetadataDto(string Icon, bool SupportsBinding = true, bool SupportsResponsiveLayout = true, List<string>? AllowedRegions = null, List<string>? Tags = null);
public sealed record ComponentDefinitionDto(Guid? Id, string Code, string Name, string Description, string Category, string RendererKey, List<ComponentPropertyDto> Properties, List<ComponentEventDto> Events, List<ComponentValidationDto> Validations, ComponentDesignMetadataDto DesignMetadata, MetadataStatus Status = MetadataStatus.Draft, int Version = 1);

[ApiController, Route("api/component-definitions")]
public sealed class ComponentDefinitionsController(EProcurementDbContext db) : ControllerBase
{
    static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [HttpGet]
    public async Task<ActionResult<List<ComponentDefinitionDto>>> List(CancellationToken ct) => Ok((await db.ComponentDefinitions.AsNoTracking().OrderBy(x => x.Category).ThenBy(x => x.Name).ToListAsync(ct)).Select(ToDto).ToList());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ComponentDefinitionDto>> Get(Guid id, CancellationToken ct) => await db.ComponentDefinitions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) is { } item ? Ok(ToDto(item)) : NotFound();

    [HttpPost]
    public async Task<ActionResult<ComponentDefinitionDto>> Create(ComponentDefinitionDto dto, CancellationToken ct)
    {
        var item = new ComponentDefinition(dto.Code, dto.Name, dto.Description, dto.Category, dto.RendererKey, Serialize(dto.Properties), Serialize(dto.Events), Serialize(dto.Validations), Serialize(dto.DesignMetadata), dto.Version, dto.Status);
        db.ComponentDefinitions.Add(item);
        await db.SaveChangesAsync(ct);
        return Created($"/api/component-definitions/{item.Id}", ToDto(item));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ComponentDefinitionDto>> Update(Guid id, ComponentDefinitionDto dto, CancellationToken ct)
    {
        var item = await db.ComponentDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return NotFound();
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.Code)] = dto.Code;
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.Name)] = dto.Name;
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.Description)] = dto.Description;
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.Category)] = dto.Category;
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.RendererKey)] = dto.RendererKey;
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.PropertiesJson)] = Serialize(dto.Properties);
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.EventsJson)] = Serialize(dto.Events);
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.ValidationJson)] = Serialize(dto.Validations);
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.DesignMetadataJson)] = Serialize(dto.DesignMetadata);
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.Status)] = dto.Status;
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.Version)] = dto.Version;
        db.Entry(item).CurrentValues[nameof(ComponentDefinition.Modified)] = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return Ok(ToDto(await db.ComponentDefinitions.AsNoTracking().SingleAsync(x => x.Id == id, ct)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var item = await db.ComponentDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return NotFound();
        db.ComponentDefinitions.Remove(item);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    static ComponentDefinitionDto ToDto(ComponentDefinition c) => new(c.Id, c.Code, c.Name, c.Description, c.Category, c.RendererKey, DeserializeList<ComponentPropertyDto>(c.PropertiesJson), DeserializeList<ComponentEventDto>(c.EventsJson), DeserializeList<ComponentValidationDto>(c.ValidationJson), Deserialize(c.DesignMetadataJson, new ComponentDesignMetadataDto("Blocks")), c.Status, c.Version);
    static string Serialize<T>(T item) => JsonSerializer.Serialize(item, JsonOptions);
    static T Deserialize<T>(string json, T fallback) => string.IsNullOrWhiteSpace(json) ? fallback : JsonSerializer.Deserialize<T>(json, JsonOptions) ?? fallback;
    static List<T> DeserializeList<T>(string json) => string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? [];
}
