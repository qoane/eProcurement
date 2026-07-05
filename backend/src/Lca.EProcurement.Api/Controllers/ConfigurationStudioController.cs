using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/configuration-studio")]
public sealed class ConfigurationStudioController(IConfigurationStudioApplicationService studio) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await studio.GetStudioAsync(ct));
    [HttpGet("business-processes")] public async Task<IActionResult> BusinessProcesses(CancellationToken ct) => Ok((await studio.GetStudioAsync(ct)).BusinessProcesses);
    [HttpPost("business-processes")] public async Task<IActionResult> CreateBusinessProcess(BusinessProcessDto dto, CancellationToken ct) => Created("/api/configuration-studio/business-processes", await studio.CreateBusinessProcessAsync(dto, ct));
    [HttpPost("business-processes/{code}/publish")] public async Task<IActionResult> PublishBusinessProcess(string code, CancellationToken ct) => (await studio.PublishBusinessProcessAsync(code, ct)) is { } item ? Ok(item) : NotFound();
    [HttpGet("document-requirement-sets")] public async Task<IActionResult> DocumentRequirementSets(CancellationToken ct) => Ok((await studio.GetStudioAsync(ct)).DocumentRequirementSets);
    [HttpPost("document-requirement-sets")] public async Task<IActionResult> CreateDocumentRequirementSet(DocumentRequirementSetDto dto, CancellationToken ct) => Created("/api/configuration-studio/document-requirement-sets", await studio.CreateDocumentRequirementSetAsync(dto, ct));
    [HttpGet("approval-matrices")] public async Task<IActionResult> ApprovalMatrices(CancellationToken ct) => Ok((await studio.GetStudioAsync(ct)).ApprovalMatrices);
    [HttpPost("approval-matrices")] public async Task<IActionResult> CreateApprovalMatrix(ApprovalMatrixDto dto, CancellationToken ct) => Created("/api/configuration-studio/approval-matrices", await studio.CreateApprovalMatrixAsync(dto, ct));
}
