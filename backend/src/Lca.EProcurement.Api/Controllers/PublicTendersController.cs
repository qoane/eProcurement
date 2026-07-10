using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/public")]
public sealed class PublicTendersController(IPublicTenderApplicationService publicTenders) : ControllerBase
{
    [HttpGet("tenders")]
    public async Task<IActionResult> GetTenders(CancellationToken ct) => Ok(await publicTenders.GetTendersAsync(ct));

    [HttpGet("tenders/{reference}")]
    public async Task<IActionResult> GetTender(string reference, CancellationToken ct) => (await publicTenders.GetTenderAsync(reference, ct)) is { } tender ? Ok(tender) : NotFound();

    [HttpGet("tender-categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct) => Ok(await publicTenders.GetCategoriesAsync(ct));

    [HttpGet("tender-calendar")]
    public async Task<IActionResult> GetCalendar(CancellationToken ct) => Ok(await publicTenders.GetCalendarAsync(ct));

    [HttpGet("widgets/latest-tenders")]
    public async Task<IActionResult> GetLatest(CancellationToken ct, [FromQuery] int take = 5) => Ok(await publicTenders.GetLatestAsync(take, ct));

    [HttpGet("awards")]
    public async Task<IActionResult> GetAwardNotices(CancellationToken ct) => Ok(await publicTenders.GetAwardNoticesAsync(ct));
}
