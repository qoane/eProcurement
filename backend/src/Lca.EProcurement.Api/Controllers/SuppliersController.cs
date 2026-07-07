using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/suppliers")]
[RequirePermission("Supplier.View")]
public sealed class SuppliersController(ISupplierApplicationService suppliers) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await suppliers.GetSuppliersAsync(ct));
    [HttpGet("registration/configuration")] public async Task<IActionResult> GetRegistrationConfiguration(CancellationToken ct) => (await suppliers.GetRegistrationConfigurationAsync(ct)) is { } c ? Ok(c) : NotFound();
    [HttpPost("registration")] public async Task<IActionResult> Register(RegisterSupplierDto dto, CancellationToken ct) => (await suppliers.RegisterAsync(dto, ct)) is { } r ? Ok(r) : NotFound();
    [HttpGet("{referenceNumber}")] public async Task<IActionResult> GetDetail(string referenceNumber, CancellationToken ct) => (await suppliers.GetSupplierDetailAsync(referenceNumber, ct)) is { } s ? Ok(s) : NotFound();
    [HttpPost("{referenceNumber}/submit")] public async Task<IActionResult> Submit(string referenceNumber, ActorDto dto, CancellationToken ct) => (await suppliers.SubmitAsync(referenceNumber, dto.Actor, ct)) is { } i ? Ok(i) : NotFound();
}
