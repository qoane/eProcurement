using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/form-definitions")]
[RequirePermission("Studio.Forms")]
public sealed class DynamicFormsController(IDynamicFormApplicationService forms) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await forms.GetDefinitionsAsync(ct));
    [HttpPost] public async Task<IActionResult> Create(CreateFormDefinitionDto dto, CancellationToken ct) => Created($"/api/form-definitions/{dto.Code}", await forms.CreateDefinitionAsync(dto, ct));
    [HttpPost("{code}/sections")] public async Task<IActionResult> AddSection(string code, FormSectionDto dto, CancellationToken ct) => (await forms.AddSectionAsync(code, dto, ct)) is { } s ? Created($"/api/form-definitions/{code}/sections/{dto.Code}", s) : NotFound();
    [HttpPost("{code}/sections/{sectionCode}/fields")] public async Task<IActionResult> AddField(string code, string sectionCode, FormFieldDto dto, CancellationToken ct) => (await forms.AddFieldAsync(code, sectionCode, dto, ct)) is { } f ? Created($"/api/form-definitions/{code}/sections/{sectionCode}/fields/{dto.Code}", f) : NotFound();
    [HttpPost("{code}/publish")] public async Task<IActionResult> Publish(string code, ActorDto dto, CancellationToken ct) => (await forms.PublishVersionAsync(code, dto.Actor, ct)) is { } f ? Ok(f) : NotFound();
    [HttpGet("{code}/active")] public async Task<IActionResult> Active(string code, CancellationToken ct) => (await forms.GetActiveByCodeAsync(code, ct)) is { } f ? Ok(f) : NotFound();
    [HttpPost("submissions")] public async Task<IActionResult> Submit(SubmitFormDto dto, CancellationToken ct) => Ok(await forms.SubmitAsync(dto, ct));
    [HttpGet("submissions/{entityType}/{entityId:guid}")] public async Task<IActionResult> Submissions(string entityType, Guid entityId, CancellationToken ct) => Ok(await forms.GetSubmissionsAsync(entityType, entityId, ct));
}
