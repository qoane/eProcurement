using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/support-cases")]
public sealed class SupportCasesController(IOperationsApplicationService ops) : ControllerBase
{
    [RequirePermission("SupportCase.View"), HttpGet] public async Task<IActionResult> Get(int page=1,int pageSize=25,string? status=null,CancellationToken ct=default)=>Ok(await ops.GetSupportCasesAsync(page,pageSize,status,ct));
    [RequirePermission("SupportCase.View"), HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id,CancellationToken ct)=>(await ops.GetSupportCaseAsync(id,ct)) is { } c ? Ok(c) : NotFound();
    [RequirePermission("SupportCase.Create"), HttpPost] public async Task<IActionResult> Create(SupportCaseCreateDto dto,CancellationToken ct)=>Ok(await ops.CreateSupportCaseAsync(dto,User.Identity?.Name??"anonymous",ct));
    [RequirePermission("SupportCase.Manage"), HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, SupportCaseCreateDto dto,CancellationToken ct)=>(await ops.GetSupportCaseAsync(id,ct)) is { } ? Ok(await ops.ResolveSupportCaseAsync(id,dto.Description,ct)) : NotFound();
    [RequirePermission("SupportCase.Manage"), HttpPost("{id:guid}/assign")] public async Task<IActionResult> Assign(Guid id,[FromBody]string assignedTo,CancellationToken ct)=>(await ops.AssignSupportCaseAsync(id,assignedTo,ct)) is { } c ? Ok(c) : NotFound();
    [RequirePermission("SupportCase.Manage"), HttpPost("{id:guid}/resolve")] public async Task<IActionResult> Resolve(Guid id,[FromBody]string notes,CancellationToken ct)=>(await ops.ResolveSupportCaseAsync(id,notes,ct)) is { } c ? Ok(c) : NotFound();
    [RequirePermission("SupportCase.Manage"), HttpPost("{id:guid}/close")] public async Task<IActionResult> Close(Guid id,CancellationToken ct)=>(await ops.CloseSupportCaseAsync(id,ct)) is { } c ? Ok(c) : NotFound();
}
