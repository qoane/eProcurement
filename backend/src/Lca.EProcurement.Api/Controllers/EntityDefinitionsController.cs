using System.Text.Json;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.Api.Controllers;

public sealed record EntityPropertyDto(string Code, string Name, string DataType, bool Required = false, bool Searchable = false, string? DefaultValue = null);
public sealed record EntityRelationshipDto(string Code, string Name, string TargetEntity, string Cardinality, bool Required = false);
public sealed record EntityValidationDto(string Code, string Name, string Expression, string Message);
public sealed record EntityDesignerDto(Guid? Id, string Code, string Name, string Description, string DisplayName, string PluralName, string DefaultSearchField, List<EntityPropertyDto> Properties, List<EntityRelationshipDto> Relationships, List<EntityValidationDto> Validations, MetadataStatus Status = MetadataStatus.Draft, int Version = 1);

[ApiController, Route("api/entity-definitions")]
public sealed class EntityDefinitionsController(EProcurementDbContext db) : ControllerBase
{
    static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [HttpGet]
    public async Task<ActionResult<List<EntityDesignerDto>>> List(CancellationToken ct) => Ok((await db.EntityDefinitions.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct)).Select(ToDto).ToList());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EntityDesignerDto>> Get(Guid id, CancellationToken ct) => await db.EntityDefinitions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) is { } item ? Ok(ToDto(item)) : NotFound();

    [HttpPost]
    public async Task<ActionResult<EntityDesignerDto>> Create(EntityDesignerDto dto, CancellationToken ct)
    {
        var entity = new EntityDefinition(dto.Code, dto.Name, dto.Description, dto.DisplayName, dto.PluralName, dto.DefaultSearchField, Serialize(dto.Properties), Serialize(dto.Relationships), Serialize(dto.Validations), dto.Version, dto.Status);
        db.EntityDefinitions.Add(entity);
        await db.SaveChangesAsync(ct);
        return Created($"/api/entity-definitions/{entity.Id}", ToDto(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EntityDesignerDto>> Update(Guid id, EntityDesignerDto dto, CancellationToken ct)
    {
        var entity = await db.EntityDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.Code)] = dto.Code;
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.Name)] = dto.Name;
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.Description)] = dto.Description;
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.DisplayName)] = dto.DisplayName;
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.PluralName)] = dto.PluralName;
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.DefaultSearchField)] = dto.DefaultSearchField;
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.PropertiesJson)] = Serialize(dto.Properties);
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.RelationshipsJson)] = Serialize(dto.Relationships);
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.ValidationsJson)] = Serialize(dto.Validations);
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.Status)] = dto.Status;
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.Version)] = dto.Version;
        db.Entry(entity).CurrentValues[nameof(EntityDefinition.Modified)] = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return Ok(ToDto(await db.EntityDefinitions.AsNoTracking().SingleAsync(x => x.Id == id, ct)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await db.EntityDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();
        db.EntityDefinitions.Remove(entity);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    static EntityDesignerDto ToDto(EntityDefinition e) => new(e.Id, e.Code, e.Name, e.Description, e.DisplayName, e.PluralName, e.DefaultSearchField, Deserialize<EntityPropertyDto>(e.PropertiesJson), Deserialize<EntityRelationshipDto>(e.RelationshipsJson), Deserialize<EntityValidationDto>(e.ValidationsJson), e.Status, e.Version);
    static string Serialize<T>(List<T> items) => JsonSerializer.Serialize(items ?? [], JsonOptions);
    static List<T> Deserialize<T>(string json) => string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? [];
}
