using Lca.EProcurement.Application;
using AppActorDto = Lca.EProcurement.Application.ActorDto;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;
[ApiController]
[Route("api/invoice-matching")]
public sealed class InvoiceMatchingController(IInvoiceMatchingApplicationService matching) : ControllerBase
{
    [HttpGet] public Task<List<Lca.EProcurement.Domain.ThreeWayMatch>> Get(CancellationToken ct)=>matching.GetAsync(ct);
    [HttpGet("{invoiceId:guid}")] public async Task<IActionResult> Get(Guid invoiceId,CancellationToken ct)=>(await matching.GetAsync(invoiceId,ct)) is { } m?Ok(m):NotFound();
    [HttpPost("{invoiceId:guid}/run")] public Task<Lca.EProcurement.Domain.ThreeWayMatch> Run(Guid invoiceId,AppActorDto dto,CancellationToken ct)=>matching.RunAsync(invoiceId,dto,ct);
    [HttpGet("{invoiceId:guid}/results")] public Task<List<Lca.EProcurement.Domain.InvoiceMatchingResult>> Results(Guid invoiceId,CancellationToken ct)=>matching.GetResultsAsync(invoiceId,ct);
}
