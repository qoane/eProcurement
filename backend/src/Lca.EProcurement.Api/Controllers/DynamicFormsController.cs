using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/form-definitions")]
public sealed class DynamicFormsController(IDynamicFormApplicationService forms) : ControllerBase
{
    [HttpPost] public async Task<IActionResult> Create(CreateFormDefinitionDto dto, CancellationToken ct) => Created($"/api/form-definitions/{dto.Code}", await forms.CreateDefinitionAsync(dto, ct));
    [HttpPost("{code}/publish")] public async Task<IActionResult> Publish(string code, ActorDto dto, CancellationToken ct) => (await forms.PublishVersionAsync(code, dto.Actor, ct)) is { } f ? Ok(f) : NotFound();
    [HttpGet("{code}/active")] public async Task<IActionResult> Active(string code, CancellationToken ct) => (await forms.GetActiveByCodeAsync(code, ct)) is { } f ? Ok(f) : NotFound();
    [HttpPost("submissions")] public async Task<IActionResult> Submit(SubmitFormDto dto, CancellationToken ct) => Ok(await forms.SubmitAsync(dto, ct));
    [HttpGet("submissions/{entityType}/{entityId:guid}")] public async Task<IActionResult> Submissions(string entityType, Guid entityId, CancellationToken ct) => Ok(await forms.GetSubmissionsAsync(entityType, entityId, ct));
}
