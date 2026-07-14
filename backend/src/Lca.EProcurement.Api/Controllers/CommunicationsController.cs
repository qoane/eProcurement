using System.Security.Claims;
using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Authorize]
public sealed class CommunicationsController(ICommunicationApplicationService communications) : ControllerBase
{
    string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Missing authenticated user identity.");
    string UserName => User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? UserId;
    Guid? SupplierId => Guid.TryParse(User.FindFirstValue("supplierId"), out var id) ? id : null;
    bool IsSupplier => User.FindFirstValue("userType") == UserType.Supplier.ToString();

    [Authorize(Policy = "Communications.View")]
    [HttpGet("api/communications/threads")]
    public Task<List<CommunicationThread>> Threads(CancellationToken ct) => communications.GetThreadsAsync(UserId, false, null, ct);

    [Authorize(Policy = "Communications.View")]
    [HttpGet("api/communications/threads/{id:guid}")]
    public async Task<IActionResult> Thread(Guid id, CancellationToken ct) => (await communications.GetThreadAsync(id, UserId, false, null, ct)) is { } t ? Ok(t) : NotFound();

    [Authorize(Policy = "Communications.Manage")]
    [HttpPost("api/communications/threads")]
    public Task<CommunicationThread> Create(CreateCommunicationThreadDto dto, CancellationToken ct) => communications.CreateThreadAsync(dto, UserId, UserName, "Internal", ct);

    [Authorize(Policy = "Communications.Manage")]
    [HttpPost("api/communications/threads/{id:guid}/messages")]
    public Task<CommunicationMessage> Message(Guid id, CreateCommunicationMessageDto dto, CancellationToken ct) => communications.AddMessageAsync(id, dto, UserId, UserName, "Internal", false, null, ct);

    [Authorize(Policy = "Communications.Manage")]
    [HttpPost("api/communications/threads/{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct) { await communications.CloseAsync(id, UserId, ct); return Ok(new { success = true }); }

    [HttpGet("api/supplier/communications")]
    public Task<List<CommunicationThread>> SupplierThreads(CancellationToken ct) { if (!IsSupplier || SupplierId is null) throw new UnauthorizedAccessException("Supplier access required."); return communications.GetThreadsAsync(UserId, true, SupplierId, ct); }

    [HttpGet("api/supplier/communications/{id:guid}")]
    public async Task<IActionResult> SupplierThread(Guid id, CancellationToken ct) { if (!IsSupplier || SupplierId is null) throw new UnauthorizedAccessException("Supplier access required."); return (await communications.GetThreadAsync(id, UserId, true, SupplierId, ct)) is { } t ? Ok(t) : NotFound(); }

    [HttpPost("api/supplier/communications/{id:guid}/messages")]
    public Task<CommunicationMessage> SupplierMessage(Guid id, CreateCommunicationMessageDto dto, CancellationToken ct) { if (!IsSupplier || SupplierId is null) throw new UnauthorizedAccessException("Supplier access required."); return communications.AddMessageAsync(id, dto with { IsInternal = false }, UserId, UserName, "Supplier", true, SupplierId, ct); }
}
