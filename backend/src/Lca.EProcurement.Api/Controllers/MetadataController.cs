using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController, Route("api/metadata/{type}")]
public sealed class MetadataController(IMetadataApplicationService metadata) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List(string type, CancellationToken ct) => Ok(await metadata.ListAsync(type, ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(string type, Guid id, CancellationToken ct) => (await metadata.GetAsync(type, id, ct)) is { } item ? Ok(item) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(string type, MetadataDefinitionDto dto, CancellationToken ct) => Created($"/api/metadata/{type}", await metadata.CreateAsync(type, dto, ct));
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(string type, Guid id, MetadataDefinitionUpdateDto dto, CancellationToken ct) => (await metadata.UpdateAsync(type, id, dto, ct)) is { } item ? Ok(item) : NotFound();
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(string type, Guid id, CancellationToken ct) => await metadata.DeleteAsync(type, id, ct) ? NoContent() : NotFound();
}
