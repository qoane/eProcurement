using System.Security.Claims;
using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/supplier-portal")]
public sealed class SupplierPortalController(ISupplierPortalApplicationService portal) : ControllerBase
{
    [HttpGet("dashboard")] public async Task<IActionResult> Dashboard(CancellationToken ct) => Ok(await portal.GetDashboardAsync(Context(), ct));
    [HttpGet("profile")] public async Task<IActionResult> Profile(CancellationToken ct) => Ok(await portal.GetProfileAsync(Context(), ct));
    [HttpPut("profile")] public async Task<IActionResult> UpdateProfile(UpdateSupplierPortalProfileDto dto, CancellationToken ct) => Ok(await portal.UpdateProfileAsync(Context(), dto, ct));
    [HttpGet("documents")] public async Task<IActionResult> Documents(CancellationToken ct) => Ok(await portal.GetDocumentsAsync(Context(), ct));
    [HttpGet("opportunities")] public async Task<IActionResult> Opportunities(CancellationToken ct) => Ok(await portal.GetOpportunitiesAsync(Context(), ct));
    [HttpGet("opportunities/{reference}")] public async Task<IActionResult> Opportunity(string reference, CancellationToken ct) => (await portal.GetOpportunityAsync(Context(), reference, ct)) is { } tender ? Ok(tender) : NotFound();
    [HttpPost("opportunities/{reference}/clarifications")] public async Task<IActionResult> AskClarification(string reference, CreateSupplierClarificationDto dto, CancellationToken ct) => Ok(await portal.AskClarificationAsync(Context(), reference, dto, ct));
    [HttpGet("clarifications")] public async Task<IActionResult> Clarifications(CancellationToken ct) => Ok(await portal.GetClarificationsAsync(Context(), ct));
    [HttpGet("bids")] public async Task<IActionResult> Bids(CancellationToken ct) => Ok(await portal.GetBidsAsync(Context(), ct));
    [HttpGet("bids/{id:guid}")] public async Task<IActionResult> Bid(Guid id, CancellationToken ct) => (await portal.GetBidAsync(Context(), id, ct)) is { } bid ? Ok(bid) : NotFound();
    [HttpPost("bids")] public async Task<IActionResult> CreateBid(CreateSupplierBidDto dto, CancellationToken ct) { var bid = await portal.CreateBidAsync(Context(), dto, ct); return Created($"/api/supplier-portal/bids/{bid.Id}", bid); }
    [HttpGet("notifications")] public async Task<IActionResult> Notifications(CancellationToken ct) => Ok(await portal.GetNotificationsAsync(Context(), ct));

    SupplierPortalUserContext Context()
    {
        var userType = User.FindFirstValue("userType");
        var supplierId = User.FindFirstValue("supplierId");
        if (userType != UserType.Supplier.ToString() || !Guid.TryParse(supplierId, out var sid)) throw new UnauthorizedAccessException("Supplier portal access requires a supplier user.");
        return new SupplierPortalUserContext(sid, User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "supplier", User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
