using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController, Route("api/entities/{entityCode}/records")]
public sealed class EntityRecordsController(IEntityRecordApplicationService records) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EntityRecordDto>>> List(string entityCode, CancellationToken ct) =>
        await records.ListAsync(entityCode, ct) is { } items ? Ok(items) : NotFound();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EntityRecordDto>> Get(string entityCode, Guid id, CancellationToken ct) =>
        await records.GetAsync(entityCode, id, ct) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<ActionResult<EntityRecordDto>> Create(string entityCode, EntityRecordMutationDto dto, CancellationToken ct)
    {
        try
        {
            var created = await records.CreateAsync(entityCode, Enrich(dto), ct);
            return created is null ? NotFound() : Created($"/api/entities/{entityCode}/records/{created.Id}", created);
        }
        catch (EntityRecordValidationException ex) { return ValidationProblem(ex); }
        catch (EntityRecordPermissionException ex) { return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails { Title = "Forbidden", Detail = ex.Message, Status = StatusCodes.Status403Forbidden }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EntityRecordDto>> Update(string entityCode, Guid id, EntityRecordMutationDto dto, CancellationToken ct)
    {
        try
        {
            return await records.UpdateAsync(entityCode, id, Enrich(dto), ct) is { } item ? Ok(item) : NotFound();
        }
        catch (EntityRecordValidationException ex) { return ValidationProblem(ex); }
        catch (EntityRecordPermissionException ex) { return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails { Title = "Forbidden", Detail = ex.Message, Status = StatusCodes.Status403Forbidden }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(string entityCode, Guid id, [FromBody] EntityRecordMutationDto? dto, CancellationToken ct)
    {
        try
        {
            return await records.DeleteAsync(entityCode, id, Enrich(dto ?? new([])), ct) switch { true => NoContent(), false => NotFound(), null => NotFound() };
        }
        catch (EntityRecordPermissionException ex) { return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails { Title = "Forbidden", Detail = ex.Message, Status = StatusCodes.Status403Forbidden }); }
    }

    EntityRecordMutationDto Enrich(EntityRecordMutationDto dto)
    {
        var roles = dto.Roles ?? Request.Headers["X-User-Roles"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        var actor = dto.Actor ?? Request.Headers["X-User"].FirstOrDefault() ?? User.Identity?.Name;
        var pageCode = dto.PageCode ?? Request.Headers["X-Page-Code"].FirstOrDefault();
        return dto with { Actor = actor, Roles = roles, PageCode = pageCode };
    }

    ActionResult ValidationProblem(EntityRecordValidationException ex)
    {
        foreach (var error in ex.Errors) ModelState.AddModelError(error.Field, error.Message);
        return ValidationProblem(ModelState);
    }
}
