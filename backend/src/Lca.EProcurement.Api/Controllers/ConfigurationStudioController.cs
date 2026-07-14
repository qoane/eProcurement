using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/configuration-studio")]
[RequirePermission("Studio.View")]
public sealed class ConfigurationStudioController(IConfigurationStudioApplicationService studio) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await studio.GetStudioAsync(ct));
    [HttpGet("areas")] public async Task<IActionResult> Areas(CancellationToken ct) => Ok(await studio.GetAreaSummariesAsync(ct));
    [HttpGet("packages")] public async Task<IActionResult> Packages(CancellationToken ct) => Ok(await studio.GetPackagesAsync(ct));
    [HttpPost("packages/export")] public async Task<IActionResult> ExportPackage(ConfigurationPackageExportRequest request, CancellationToken ct) => Ok(await studio.ExportPackageAsync(request, ct));
    [HttpPost("packages/import")] public async Task<IActionResult> ImportPackage(JsonElement package, CancellationToken ct) => Ok(await studio.ImportPackageAsync(package, User.Identity?.Name ?? "system", ct));
    [HttpGet("packages/{id:guid}/download")] public async Task<IActionResult> DownloadPackage(Guid id, CancellationToken ct) => (await studio.DownloadPackageAsync(id, ct)) is { } json ? File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", $"configuration-package-{id}.json") : NotFound();
    [HttpPost("preview/{kind}")] public async Task<IActionResult> Preview(string kind, JsonElement payload, CancellationToken ct) => Ok(await studio.PreviewAsync(kind, payload, ct));
    [HttpPost("publish/{entityType}/{codeOrId}")] public async Task<IActionResult> Publish(string entityType, string codeOrId, PublishRequest request, CancellationToken ct) => Ok(await studio.PublishAsync(entityType, codeOrId, request, ct));
    [HttpGet("business-processes")] public async Task<IActionResult> BusinessProcesses(CancellationToken ct) => Ok((await studio.GetStudioAsync(ct)).BusinessProcesses);
    [HttpPost("business-processes")] public async Task<IActionResult> CreateBusinessProcess(BusinessProcessDto dto, CancellationToken ct) => Created("/api/configuration-studio/business-processes", await studio.CreateBusinessProcessAsync(dto, ct));
    [HttpPost("business-processes/{code}/publish")] public async Task<IActionResult> PublishBusinessProcess(string code, CancellationToken ct) => (await studio.PublishBusinessProcessAsync(code, ct)) is { } item ? Ok(item) : NotFound();
    [HttpGet("document-requirement-sets")] public async Task<IActionResult> DocumentRequirementSets(CancellationToken ct) => Ok((await studio.GetStudioAsync(ct)).DocumentRequirementSets);
    [HttpPost("document-requirement-sets")] public async Task<IActionResult> CreateDocumentRequirementSet(DocumentRequirementSetDto dto, CancellationToken ct) => Created("/api/configuration-studio/document-requirement-sets", await studio.CreateDocumentRequirementSetAsync(dto, ct));
    [HttpGet("approval-matrices")] public async Task<IActionResult> ApprovalMatrices(CancellationToken ct) => Ok((await studio.GetStudioAsync(ct)).ApprovalMatrices);
    [HttpPost("approval-matrices")] public async Task<IActionResult> CreateApprovalMatrix(ApprovalMatrixDto dto, CancellationToken ct) => Created("/api/configuration-studio/approval-matrices", await studio.CreateApprovalMatrixAsync(dto, ct));
}
