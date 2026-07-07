using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/tenders")]
[RequirePermission("Tender.View")]
public sealed class TendersController(ITenderApplicationService tenders) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await tenders.GetTendersAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await tenders.GetTenderAsync(id, ct)) is { } t ? Ok(t) : NotFound();
    [RequirePermission("Tender.Create")]
    [HttpPost] public async Task<IActionResult> Create(CreateTenderDto dto, CancellationToken ct) => Ok(await tenders.CreateTenderAsync(dto, ct));
    [RequirePermission("Tender.Publish")]
    [HttpPost("{id:guid}/publish")] public async Task<IActionResult> Publish(Guid id, TenderActorDto dto, CancellationToken ct) => (await tenders.PublishTenderAsync(id, dto, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id, TenderActorDto dto, CancellationToken ct) => (await tenders.CancelTenderAsync(id, dto, ct)) is { } t ? Ok(t) : NotFound();
    [HttpGet("{id:guid}/clarifications")] public async Task<IActionResult> Clarifications(Guid id, CancellationToken ct) => Ok(await tenders.GetClarificationsAsync(id, ct));
    [HttpPost("{id:guid}/clarifications")] public async Task<IActionResult> CreateClarification(Guid id, CreateTenderClarificationDto dto, CancellationToken ct) => (await tenders.CreateClarificationAsync(id, dto, ct)) is { } c ? Ok(c) : NotFound();
    [HttpPost("{id:guid}/clarifications/{clarificationId:guid}/respond")] public async Task<IActionResult> Respond(Guid id, Guid clarificationId, RespondToTenderClarificationDto dto, CancellationToken ct) => (await tenders.RespondToClarificationAsync(id, clarificationId, dto, ct)) is { } r ? Ok(r) : NotFound();
}
